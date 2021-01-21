using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	public GameObject playerDestroyEffect;
	private PlayerScript player;
	public LabirinthManager mazeScript;
	public GameObject target;

	private NavMeshAgent navMeshAgent;

	private int randomIndex;

	private void Start()
	{
		//get our PlayerScript component
		player = GameObject.Find("Player").GetComponent<PlayerScript>();

		//get our Labirinth manager component
		mazeScript = GameObject.Find("Labirinth Manager").GetComponent<LabirinthManager>();

		//get our NavMeshAgent component
		navMeshAgent = GetComponent<NavMeshAgent>();

		//Set a random destination for our enemy
		SetDestination();
	}

	private void Update()
	{
		//if the enemy is getting close to it's destination we set another destination, this way the enemy will keep moving randomly forever
		if (navMeshAgent.remainingDistance <= 1f)
		{
			SetDestination();
		}

		//if our maze is not complete we destroy our enemies
		if (!mazeScript.MazeComplete)
		{
			Destroy(gameObject);
		}
	}

	void SetDestination()
	{
		//get a random integer from 0 untill the number of cells of our maze
		randomIndex = Random.Range(0, mazeScript.Rows * mazeScript.Columns);

		//make this random integer as the index of the targets list and set the destination of our NavMeshAgent as this position
		navMeshAgent.SetDestination(mazeScript.targets[randomIndex].position);
	}

	private void OnCollisionEnter(Collision collision)
	{
		//if the enemy collides with the player
		if (collision.gameObject.tag == "Player")
		{
			//Instantiate a particle effect and we say the player is dead
			Instantiate(playerDestroyEffect, collision.gameObject.transform.position, Quaternion.identity);
			player.PlayerIsDead();
		}
	}
}
