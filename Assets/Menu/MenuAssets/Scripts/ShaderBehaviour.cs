using UnityEngine;
using System.Collections;

public class ShaderBehaviour : MonoBehaviour {

	public Material shaderEffect;
	/*private float startTime;
	private float ellapsedTime;*/

	private int _currentBeat = 0;
	private int _prevBeat = 0;
	private double _startTime = -1.0F;
	private bool _running = false;
	private bool _isBeatOne = false;

	public PostEffectScript shader;


	void Awake(){
		Init();
	}

	void Init()
	{
		/*startTime = Time.time;
		TimerStart();*/
		_startTime = AudioSettings.dspTime;
	}
	// Update is called once per frame
	void Update () {
		/*
		_prevBeat = _currentBeat;
		_currentBeat = (int)((AudioSettings.dspTime - _startTime) / (60.0F)) % 5;

		_isBeatOne = (_prevBeat == 5 - 1 && _currentBeat == 0);

		Debug.Log ("second::" + _currentBeat + ";;" + (AudioSettings.dspTime - _startTime));

		// this only enteres the loop on a beat (when there is a change of beat)
		if (_prevBeat != _currentBeat) {
			if (_currentBeat == 0) {
				Debug.Log ("!!!!!!!!!!!!Shader");
				//shader.ChangeEffect ();
			}
		}*/
		/*TimerStart();

		float minutes = Mathf.Floor (ellapsedTime / 60);
		float seconds = Mathf.Floor (ellapsedTime - minutes * 60);

		// wait for 5 seconds
		// switch shader on
		// wait for 5 seconds
		// switch shader off
		Debug.Log ("second::" + seconds + ":: SECONDS" + (int)seconds % 5);
		if((int)seconds > 5){
			Debug.Log ("!!!!!!!!!!!!Shader");


		}*/
	}

	/*void TimerStart(){
		ellapsedTime = Time.time - startTime;
	}*/
}
