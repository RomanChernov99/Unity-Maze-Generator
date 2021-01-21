using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Elements : MonoBehaviour
{
	public LabirinthManager MazeScript;
	public FInishLineScript VictoryScript;
	private PlayerScript player;
	public CanvasGroup canvasGroup;

	public GameObject ResetButton;
	public GameObject Regenerate_Hunt_And_Kill;
	public GameObject Regenerate_Aldous_Broder;
	public GameObject VictoryText;
	public GameObject PlayAgainButton;

	public Animator UIAnimator;

	private void Update()
	{
		//When we have a complete maze, we animate our UI Elements fading out and scale up our reset button
		if (MazeScript.MazeComplete)
		{
			UIAnimator.SetBool("UIFadeOut", true);

			//Make our UI Elements non interactable
			canvasGroup.interactable = false;

			LeanTween.scale(ResetButton, new Vector3(1.5f, 1.5f, 1.5f), 1f);
		}

		//If we reset the maze which means we didn't win and the maze is not complete we scale down the reset button
		else if (!MazeScript.MazeComplete && !VictoryScript.YouWin)
		{
			//Make our UI Elements interactable again
			canvasGroup.interactable = true;

			LeanTween.scale(ResetButton, new Vector3(0, 0, 0), 1f);
		}

		//If we win we scale up our victory text and the play again button and scale down the reset button
		if (VictoryScript.YouWin)
		{
			LeanTween.scale(VictoryText, new Vector3(1, 1, 1), 1f);
			LeanTween.scale(PlayAgainButton, new Vector3(1, 1, 1), 1f);

			LeanTween.scale(ResetButton, new Vector3(0, 0, 0), 1f);
		}

		//If we used the Hunt And Kill Algorithm we scale up the button that regenerates the maze using this Algorithm
		if (MazeScript.Regenerate_Hunt_And_Kill)
		{
			LeanTween.scale(Regenerate_Hunt_And_Kill, new Vector3(1.5f, 1.5f, 1.5f), 1f);
		}

		//If not then this button should be scaled down
		else
		{
			LeanTween.scale(Regenerate_Hunt_And_Kill, new Vector3(0, 0, 0), 1f);
		}

		//If we used the Aldous Broder Algorithm we scale up the button that regenerates the maze using this Algorithm
		if (MazeScript.Regenerate_Aldous_Broder)
		{
			LeanTween.scale(Regenerate_Aldous_Broder, new Vector3(1.5f, 1.5f, 1.5f), 1f);
		}

		//If not then this button should be scaled down
		else
		{
			LeanTween.scale(Regenerate_Aldous_Broder, new Vector3(0, 0, 0), 1f);
		}

		//if our maze is complete and we don't have our player
		if (MazeScript.MazeComplete && !player)
		{
			//start this coroutine which waits for our player to be instantiated before getting it's PlayerScript component
			StartCoroutine(WaitForPlayerInstantiate());
		}

		//now if we are playing and our player dies
		else if (MazeScript.MazeComplete && player.isDead)
		{
			//we delete the labirinth and reset the UI Elements
			MazeScript.EraseLab();
			SetAnimatorBoolToFalse();
		}

		else
		{
			return;
		}

	}

	public void PlayAgain()
	{
		SetAnimatorBoolToFalse();

		//reset our YouWin boolean
		VictoryScript.YouWin = false;

		//Scale back up every UI Element and scale down the victory text and the play again button
		foreach( Transform transform in transform)
		{
			LeanTween.scale(VictoryText, new Vector3(0, 0, 0), 1f);
			LeanTween.scale(PlayAgainButton, new Vector3(0, 0, 0), 1f);
		}
	}

	public void SetAnimatorBoolToFalse()
	{
		//We want our UIElements to fade back in when we press play again or reset button
		UIAnimator.SetBool("UIFadeOut", false);
	}

	IEnumerator WaitForPlayerInstantiate()
	{
		//wait untill our player is instantiated so we can get it's PlayerScript component
		yield return new WaitForSecondsRealtime(2.5f);

		if (GameObject.Find("Player"))
		{
			//get the player's PlayerScript component if we can find a Player on our scene
			player = GameObject.Find("Player").GetComponent<PlayerScript>();
		}
	}

	//quit the game
	public void QuitGame()
	{
		Debug.Log("Exiting game...");
		Application.Quit();
	}
}
