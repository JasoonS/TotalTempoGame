using UnityEngine;
using System.Collections;

public class GameOverScript : MonoBehaviour {

    private static GameOverScript _gameOverManager;
    private GameObject _gameObject;
    private Canvas _canvas;

    // Use the static object pattern to guarantee that this object is correctly assigned and pressent in the scene.
    public static GameOverScript Instance
    {
        get
        {
            if (!_gameOverManager)
            {
                _gameOverManager = FindObjectOfType(typeof(GameOverScript)) as GameOverScript;

                if (!_gameOverManager)
                {
                    Debug.LogError("You need to have at least one active 'GameOver Menu' script in your scene.");
                }
                else
                {
                    _gameOverManager.Init();
                }
            }
            return _gameOverManager;
        }
    }

    void Start()
    {
        Init();
        Debug.Log("Initialised");
    }

    void Init()
    {
        _canvas = GetComponent<Canvas>();

        //_gameObject.SetActive (false);
        //_gameObject.enabled = false;
        _canvas.enabled = false;
    }

    public static void EnableGameOverMenu(bool active, string message) {
        //Instance._gameObject.SetActive(active);
        if (!Instance._canvas.enabled)
            Instance._canvas.enabled = true;
    }
}
