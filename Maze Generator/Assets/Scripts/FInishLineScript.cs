using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FInishLineScript : MonoBehaviour
{
	public LabirinthManager MazeScript;

	public bool YouWin = false;


	private void Update()
	{
		//If we are still playing the YouWin boolean should be false
		if (MazeScript.MazeComplete)
		{
			YouWin = false;
		}
	}

	//when our player crosses the finish line we win and erase our lab if there are no more enemies 
	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			if (!GameObject.Find("Enemy"))
			{
				YouWin = true;
				MazeScript.EraseLab();
			}

			else
			{
				Debug.Log("There are still enemies to be eliminated");
			}
		}
	}
}
