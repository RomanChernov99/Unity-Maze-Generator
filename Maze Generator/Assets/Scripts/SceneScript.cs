using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class SceneScript : MonoBehaviour
{
	public Animator SceneTransition;

	public GameObject HowToPlayText;
	public GameObject ControlsText;
	public GameObject WelcomeText;
	public GameObject ContinueButton;
	public GameObject Play_Button;

	public void Continue()
	{
		//Animate our UI using LeanTween.scale 
		LeanTween.scale(WelcomeText, new Vector3(0, 0, 0), 1f).setDestroyOnComplete(true);
		LeanTween.scale(ContinueButton, new Vector3(0, 0, 0), 1f).setDestroyOnComplete(true);

		LeanTween.scale(Play_Button, new Vector3(1, 2, 1), 1f);
		LeanTween.scale(HowToPlayText, new Vector3(2, 2, 2), 1f);
		LeanTween.scale(ControlsText, new Vector3(1, 1, 1), 1f);
	}

	public void PlayButton()
	{
		StartCoroutine(LoadGameScene());
	}

	//simple couroutine that animates our scene transition and loads the Game Scene after 1 second has passed
	IEnumerator LoadGameScene()
	{
		SceneTransition.SetTrigger("Transition");

		yield return new WaitForSecondsRealtime(1f);

		SceneManager.LoadScene("Game Scene", LoadSceneMode.Single);
	}
}
