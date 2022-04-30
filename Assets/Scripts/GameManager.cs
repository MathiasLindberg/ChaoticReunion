using System;
using UnityEngine;
using System.Collections;

public enum GameStates
{
    Default,
    Initialising,
    Starting,
    Running,
    Finishing
}

public class GameManager : MonoBehaviour
{
    public GameStates GameState { get; set; }
    
    public static GameManager Instance
    {
        get
        {
            if (_instance) return _instance;
            
            _instance = FindObjectOfType<GameManager>();
            if (!_instance)
                _instance = new GameObject("GameManager").AddComponent<GameManager>();

            return _instance;
        }
    }
    
    private static GameManager _instance;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(_instance);
    }

    private void Start()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(GameInitialising());
        yield return StartCoroutine(GameStarting());
        yield return StartCoroutine(GameRunning());
        yield return StartCoroutine(GameFinishing());
    }

    private IEnumerator GameInitialising()
    {
        GameState = GameStates.Initialising;
        Debug.Log("Current Game State: " + GameState);
        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator GameStarting()
    {
        GameState = GameStates.Starting;
        Debug.Log("Current Game State: " + GameState);
        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator GameRunning()
    {
        GameState = GameStates.Running;
        Debug.Log("Current Game State: " + GameState);
        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator GameFinishing()
    {
        GameState = GameStates.Finishing;
        Debug.Log("Current Game State: " + GameState);
        yield return null;
    }
}
