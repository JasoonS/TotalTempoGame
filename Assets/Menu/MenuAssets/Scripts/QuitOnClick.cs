using UnityEngine;
using System.Collections;

public class QuitOnClick : MonoBehaviour 
{
	public void Quit()
	{
		UnityEditor.EditorApplication.isPlaying = false;
		Application.Quit();
	}
}
