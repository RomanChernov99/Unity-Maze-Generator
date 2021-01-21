using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DontDestroyAudioSource : MonoBehaviour
{
	//Simply keep our background music playing after we change scenes
	private void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
	}
}
