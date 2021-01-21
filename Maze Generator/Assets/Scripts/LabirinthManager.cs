using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class LabirinthManager : MonoBehaviour
{
	public GameObject Floor;
	public GameObject Wall;
	public GameObject FinishLine;
	public GameObject Enemy;
	public GameObject target;
	public GameObject targetsParent;
	public GameObject Player;

	public bool MazeComplete = false;
	public bool Regenerate_Hunt_And_Kill = false;
	public bool Regenerate_Aldous_Broder = false;

	public int Rows;
	public int Columns;

	public InputField Height;
	public InputField Width;

	public MazeCell[,] labirinth;
	public List<Transform> targets;

	private int current_Row = 0;
	private int current_Column = 0;

	private float size = 5f;

	private NavMeshSurface Nav_Mesh_Surface;

	private WaitForSeconds waitForSeconds;

	private void Start()
	{
		//Get our Nav Mesh Surface Component
		Nav_Mesh_Surface = GetComponent<NavMeshSurface>();
	}

	public void GenerateGrid()
	{
		//make our rows and columns value the ones that the player inputs through the input field
		int rows;
		int columns;

		if (int.TryParse(Height.text, out rows))
		{
			Rows = rows;
		}
		if (int.TryParse(Width.text, out columns))
		{
			Columns = columns;
		}

		//Delete last labirinth generated
		EraseLab();

		//create the labirinth
		labirinth = new MazeCell[Rows, Columns];

		//initialize our list
		targets = new List<Transform>();

		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
			{
				//create a new cell
				labirinth[i, j] = new MazeCell();

				//Instantiate our targets centered on each cell, these are for our enemy AI
				GameObject targetPos = Instantiate(target, new Vector3(j * size, 1, -i * size), Quaternion.identity, targetsParent.transform);

				//here we get a list of all targets
				targets.Add(targetPos.transform);


				//Instantiate every gameobject of our maze and animate them scaling up

				GameObject floor = Instantiate(Floor, new Vector3(j * size, 0, -i * size), Quaternion.identity);
				floor.name = "Floor " + i + "_" + j;

				LeanTween.scale(floor, new Vector3(5, 1, 5), 1f);


				GameObject upWall = Instantiate(Wall, new Vector3(j * size, 1f, -i * size + 2), Quaternion.Euler(90, 0, 0));
				upWall.name = "UpWall" + i + "_" + j;

				LeanTween.scale(upWall, new Vector3(5, 1, 5), 1f);


				GameObject downWall = Instantiate(Wall, new Vector3(j * size, 1, -i * size - 2), Quaternion.Euler(90, 0, 0));
				downWall.name = "DownWall" + i + "_" + j;

				LeanTween.scale(downWall, new Vector3(5, 1, 5), 1f);


				GameObject rightWall = Instantiate(Wall, new Vector3(j * size + 2, 1f, -i * size), Quaternion.Euler(0, 0, 90));
				rightWall.name = "RightWall" + i + "_" + j;

				LeanTween.scale(rightWall, new Vector3(5, 1, 5), 1f);


				GameObject leftWall = Instantiate(Wall, new Vector3(j * size - 2, 1f, -i * size), Quaternion.Euler(0, 0, 90));
				leftWall.name = "LeftWall" + i + "_" + j;

				LeanTween.scale(leftWall, new Vector3(5, 1, 5), 1f);


				//assign each game object to its correct reference in our MazeCell Class
				labirinth[i, j].UpWall = upWall;
				labirinth[i, j].DownWall = downWall;
				labirinth[i, j].RightWall = rightWall;
				labirinth[i, j].LeftWall = leftWall;


				//assign the parent of these game objects for organization in the hierarchy
				floor.transform.parent = transform;
				upWall.transform.parent = transform;
				downWall.transform.parent = transform;
				rightWall.transform.parent = transform;
				leftWall.transform.parent = transform;


				//remove top left wall and bottom right wall to create an entrance and an exit and adjust our finish line game object's position
				if (i == 0 && j == 0)
				{
					leftWall.SetActive(false);
				}
				if (i == Rows - 1 && j == Columns - 1)
				{
					rightWall.SetActive(false);

					//adjust our finish line position if Rows = Columns
					FinishLine.transform.position = new Vector3(j * size + 3f, 1.5f, -i * size);
				}

				//adjust our finish line position if Rows != Columns
				if (i == Rows - 1 && j == Columns - 1 && i != j)
				{
					FinishLine.transform.position = new Vector3(j * size + 3, 1.5f, -i * size);
				}
			}
		}

		//adjust camera to fit in the entire maze
		AdjustCameraPosition();
	}


	// here we have our two main algorithms for generating perfect mazes

	//HuntAndKill Algorithm
	#region Hunt And Kill Alorithm

	public void HuntAndKill()
	{
		//Generate our maze
		GenerateGrid();

		//first we mark our first cell which is the top left cell as visited
		labirinth[current_Row, current_Column].Visited = true;

		//now we will run the Hunt And Kill algorithm while we still have unvisited cells
		while (!EveryCellIsVisited())
		{
			HuntAndKillAlgorithm();
			ScanGrid();
		}

		//After we complete our maze we set this bool to true
		MazeComplete = true;

		//Start this couroutine which allows for the LeanTween animation to finish before we bake our navmesh and instantiate our players
		StartCoroutine(WaitForAnimation());

		//Set these booleans for UI Purposes
		Regenerate_Hunt_And_Kill = true;
		Regenerate_Aldous_Broder = false;
	}


	void HuntAndKillAlgorithm()
	{
		//while we have an unvisited neighbour
		while (HasUnvisitedNeighbours())
		{
			//we choose a random direction
			int direction = Random.Range(0, 4);

			//for each cell we need to check in all directions if the cell is visited or not and destroy the walls between the unvisited neighbour and our current cell

			//up
			if (direction == 0)
			{
				//if we are checking any row other than the first row
				if (current_Row > 0 && !labirinth[current_Row - 1, current_Column].Visited)
				{
					//destroy first wall
					if (labirinth[current_Row, current_Column].UpWall)
					{
						labirinth[current_Row, current_Column].UpWall.SetActive(false);
					}

					//update current Row
					current_Row--;

					//mark as visited
					labirinth[current_Row, current_Column].Visited = true;

					//destroy second wall to open up a path between the two cells
					if (labirinth[current_Row, current_Column].DownWall)
					{
						labirinth[current_Row, current_Column].DownWall.SetActive(false);
					}
				}
			}

			//down
			else if (direction == 1)
			{
				//if we are checking any row other than the last row

				if (current_Row < Rows - 1 && !labirinth[current_Row + 1, current_Column].Visited)
				{
					//destroy the first wall
					if (labirinth[current_Row, current_Column].DownWall)
					{
						labirinth[current_Row, current_Column].DownWall.SetActive(false);
					}

					//update current Row
					current_Row++;

					//mark as visited
					labirinth[current_Row, current_Column].Visited = true;

					//destroy second wall to open up a path between the two cells
					if (labirinth[current_Row, current_Column].UpWall)
					{
						labirinth[current_Row, current_Column].UpWall.SetActive(false);
					}
				}
			}

			//right
			else if (direction == 2)
			{
				//if we are checking any column other than the last one
				if (current_Column < Columns - 1 && !labirinth[current_Row, current_Column + 1].Visited)
				{
					//destroy first wall
					if (labirinth[current_Row, current_Column].RightWall)
					{
						labirinth[current_Row, current_Column].RightWall.SetActive(false);
					}

					//update current Column
					current_Column++;

					//mark as visited
					labirinth[current_Row, current_Column].Visited = true;

					//destroy second wall to open up a path between the two cells
					if (labirinth[current_Row, current_Column].LeftWall)
					{
						labirinth[current_Row, current_Column].LeftWall.SetActive(false);
					}
				}
			}

			//left
			else if (direction == 3)
			{
				//if we are checking any column other than the first one
				if (current_Column > 0 && !labirinth[current_Row, current_Column - 1].Visited)
				{
					//destroy the first wall
					if (labirinth[current_Row, current_Column].LeftWall)
					{
						labirinth[current_Row, current_Column].LeftWall.SetActive(false);
					}

					//update current Column
					current_Column--;

					//mark as visited
					labirinth[current_Row, current_Column].Visited = true;

					//destroy second wall to open up a path
					if (labirinth[current_Row, current_Column].RightWall)
					{
						labirinth[current_Row, current_Column].RightWall.SetActive(false);
					}
				}
			}
		}
	}


	//After we reach a dead end where all our neighbours are visited we call the ScanGrid Method
	void ScanGrid()
	{
		//here we want to scan our labirinth and check to see if we can find any cell that is unvisited and has a visited neighbour cell
		//and set that as our starting cell

		//for every cell
		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
			{
				//if its not visited and has a visited neighbour
				if (!labirinth[i, j].Visited && HasVisitedNeighbour(i, j))
				{
					//we update our currentRow and currentColumn
					current_Row = i;
					current_Column = j;

					//mark this cell as visited
					labirinth[current_Row, current_Column].Visited = true;

					//destroy a wall to and adjacent visited neighbour of this new cell and use the HuntAndKillAlgorithm Method again and return
					RemoveAdjacentWall();
					HuntAndKillAlgorithm();
					return;
				}
			}
		}
	}

	#endregion


	//Aldous Broder Algorithm
	#region Aldous Broder Algorithm

	public void Aldous_Broder()
	{
		//create our maze
		GenerateGrid();

		//first we choose a random cell to be our starting position
		current_Row = Random.Range(0, Rows);
		current_Column = Random.Range(0, Columns);

		//mark this cell as visited
		labirinth[current_Row, current_Column].Visited = true;

		//while we have unvisited cells
		while (!EveryCellIsVisited())
		{
			//we run the Aldous Broder Algorithm
			AldousBroderAlgorithm();
		}

		//After we complete our maze we set this bool to true
		MazeComplete = true;

		//wait for leantween animation to finish before we bake our navmesh and instantiate our player
		StartCoroutine(WaitForAnimation());

		//Set these booleans for UI Purposes
		Regenerate_Aldous_Broder = true;
		Regenerate_Hunt_And_Kill = false;
	}
 
	void AldousBroderAlgorithm()
	{
		//take a random neighbour
		int neighbour = Random.Range(0, 4);

		//In this Algorithm we update our current cell everytime we choose a neighbour even if it's already visited, making this Algorithm a little bit slower than the previous

		//neighbour above current cell
		if (neighbour == 0 && current_Row > 0)
		{
			//update our current cell
			current_Row--;

			//if this new cell is unvisited we open a path to the previous cell
			if (!labirinth[current_Row, current_Column].Visited)
			{
				//destroy the downwall of our new current cell
				labirinth[current_Row, current_Column].DownWall.SetActive(false);

				//destroy the upwall of our previous cell
				labirinth[current_Row + 1, current_Column].UpWall.SetActive(false);

				//mark this new cell as visited
				labirinth[current_Row, current_Column].Visited = true;
			}
		}

		//neighbour below current cell
		if (neighbour == 1 && current_Row < Rows - 1)
		{
			//update our current cell
			current_Row++;

			//if this new cell is unvisited we open a path to the previous cell
			if (!labirinth[current_Row, current_Column].Visited)
			{
				//destroy the upwall of our new current cell
				labirinth[current_Row, current_Column].UpWall.SetActive(false);

				//destroy the downwall of our previous cell
				labirinth[current_Row - 1, current_Column].DownWall.SetActive(false);

				//mark this new cell as visited
				labirinth[current_Row, current_Column].Visited = true;
			}
		}

		//neighbour to the right of current cell
		else if (neighbour == 2 && current_Column < Columns - 1)
		{
			//update our current cell
			current_Column++;

			//if this new cell is unvisited we open a path to the previous cell
			if (!labirinth[current_Row, current_Column].Visited)
			{
				//destroy the leftwall of our new current cell
				labirinth[current_Row, current_Column].LeftWall.SetActive(false);

				//destroy the right wall of our previous cell
				labirinth[current_Row, current_Column - 1].RightWall.SetActive(false);

				//mark this new cell as visited
				labirinth[current_Row, current_Column].Visited = true;
			}
		}

		//neighbour to the left of current cell
		if (neighbour == 3 && current_Column > 0)
		{
			//update our current cell
			current_Column--;

			//if this new cell is unvisited we open a path to the previous cell
			if (!labirinth[current_Row, current_Column].Visited)
			{
				//destroy the rightwall of our new current cell
				labirinth[current_Row, current_Column].RightWall.SetActive(false);

				//destroy the leftwall of our previous cell
				labirinth[current_Row, current_Column + 1].LeftWall.SetActive(false);

				//mark this new cell as visited
				labirinth[current_Row, current_Column].Visited = true;
			}
		}
	}
	#endregion


	//These next functions help our algorithms work well and some for gameplay aspects
	#region Other Functions

	public void EraseLab()
	{
		//reset our camera Field Of View
		Camera.main.fieldOfView = 60f;

		//find our player
		GameObject player = GameObject.Find("Player"); 

		//animate our player fading out and destroy it, if there is a player in the scene
		if (player)
		{
			LeanTween.scale(player, new Vector3(0, 0, 0), 0.5f).setDestroyOnComplete(true);
		}

		//animate every gameobject of our maze fading out and destroy them
		foreach (Transform transform in transform)
		{
			LeanTween.scale(transform.gameObject, new Vector3(0, 0, 0), 0.6f).setDestroyOnComplete(true);
		}

		//reset our current cell
		current_Row = 0;
		current_Column = 0;

		//Set this boolean to false
		MazeComplete = false;

		//Set these booleans for UI Purposes
		Regenerate_Hunt_And_Kill = false;
		Regenerate_Aldous_Broder = false;
	}

	void RemoveAdjacentWall()
	{
		//here we take a random direction and check to see if we have a visited neighbour if we do we destroy the walls in between
		bool removed = false;

		while (!removed)
		{

			int direction = Random.Range(0, 4);

			//up
			if (direction == 0)
			{
				if (current_Row > 0 && labirinth[current_Row - 1, current_Column].Visited)
				{
					labirinth[current_Row - 1, current_Column].DownWall.SetActive(false);
					labirinth[current_Row, current_Column].UpWall.SetActive(false);
					removed = true;
				}
			}

			//down
			else if (direction == 1)
			{
				if (current_Row < Rows - 1 && labirinth[current_Row + 1, current_Column].Visited)
				{
					labirinth[current_Row + 1, current_Column].UpWall.SetActive(false);
					labirinth[current_Row, current_Column].DownWall.SetActive(false);
					removed = true;
				}
			}

			//right
			else if (direction == 2)
			{
				if (current_Column < Columns - 1 && labirinth[current_Row, current_Column + 1].Visited)
				{
					labirinth[current_Row, current_Column + 1].LeftWall.SetActive(false);
					labirinth[current_Row, current_Column].RightWall.SetActive(false);
					removed = true;
				}
			}

			//left
			else if (direction == 3)
			{
				if (current_Column > 0 && labirinth[current_Row, current_Column - 1].Visited)
				{
					labirinth[current_Row, current_Column - 1].RightWall.SetActive(false);
					labirinth[current_Row, current_Column].LeftWall.SetActive(false);
					removed = true;
				}
			}
		}
	}

	void AdjustCameraPosition()
	{
		//here we want our Camera to adjust to the size of the maze

		//get our camera position
		Vector3 cameraPos = Camera.main.transform.position;

		//update our camera position with respect to the number of columns and rows of the maze
		cameraPos.x = (Columns / 2) * size - 2.5f;
		cameraPos.y = Mathf.Max(30, Mathf.Max(Rows, Columns) * 6f);
		cameraPos.z = -(Rows / 2) * size + 2.5f;

		//adjust camera position
		Camera.main.transform.position = cameraPos;
	}

	IEnumerator WaitForAnimation()
	{
		//wait for LeanTween Animation to finish 
		yield return new WaitForSecondsRealtime(2f);

		//build our Nav Mesh
		Nav_Mesh_Surface.BuildNavMesh();

		//instantiate our player
		GameObject player = Instantiate(Player, new Vector3(0, 1.5f, 0), Quaternion.identity);
		player.name = "Player";

		//Instantiate our Enemies
		InstantiateEnemy();

		//animate our player instantiating
		LeanTween.scale(player, new Vector3(1.5f, 1.5f, 1.5f), 0.5f);

	}

	void InstantiateEnemy()
	{
		//Here there is a 1/6 chance of Instantiating one enemy per cell
		//So if we make a small maze there will likely be at most 1 or 2 enemies but if we make a huge maze we will have a lot of them
		//for every cell except the first few around the starting cell
		for (int i = 2; i < Rows; i++)
		{
			for (int j = 2; j < Columns; j++)
			{
				//take a random integer from 1-6
				int random = Random.Range(1, 7);

				//Only if it's 1
				if (random == 1)
				{
					//we Instantiate one enemy
					GameObject enemy = Instantiate(Enemy, new Vector3(j * size, 1.5f, -i * size), Quaternion.identity);

					//set it's name to Enemy
					enemy.name = "Enemy";

					//set it's transform parent
					enemy.transform.parent = transform;
				}
			}
		}
	}
	#endregion


	//Necessary boolean functions for our algorithms

	#region Booleans

	bool HasVisitedNeighbour(int row, int column)
	{
		//this boolean returns true in the case of our current cell having a visited neighbour

		//cell above current cell
		if (row > 0 && labirinth[row - 1, column].Visited)
		{
			return true;
		}

		//cell below current cell
		if (row < Rows - 1 && labirinth[row + 1, column].Visited)
		{
			return true;
		}

		//cell to the right of current cell
		if (column < Columns - 1 && labirinth[row, column + 1].Visited)
		{
			return true;
		}

		//cell to the left of current cell
		if (column > 0 && labirinth[row, column - 1].Visited)
		{
			return true;
		}

		return false;
	}

	bool HasUnvisitedNeighbours()
	{
		//here for every neighbour cell we return true if any neighbour cell is unvisited

		//up
		if (IsCellUnvisited(current_Row - 1, current_Column))
		{
			return true;
		}

		//down
		if (IsCellUnvisited(current_Row + 1, current_Column))
		{
			return true;
		}

		//left
		if (IsCellUnvisited(current_Row, current_Column - 1))
		{
			return true;
		}

		//right
		if (IsCellUnvisited(current_Row, current_Column + 1))
		{
			return true;
		}

		return false;
	}

	bool IsCellUnvisited(int row, int column)
	{
		//if the cell is within the boundaries of our maze and it is not visited we return true
		if (row >= 0 && row <= Rows - 1 && column >= 0 && column <= Columns - 1 && !labirinth[row, column].Visited)
		{
			return true;
		}
		return false;
	}

	bool EveryCellIsVisited()
	{
		//check every cell if it's visited
		for (int i = 0; i < Rows; i++)
		{
			for (int j = 0; j < Columns; j++)
			{
				if (!labirinth[i, j].Visited)
				{
					//simply return false when there is an unvisited cell
					return false;
				}
			}
		}

		//if not, that means our maze is complete
		Debug.Log("Maze Complete");

		return true;
	}

	#endregion
}

