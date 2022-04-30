using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

    [Header("Gameplay Elements")] 
    [SerializeField] private List<Player> players = new List<Player>();
    [SerializeField] private Spawner spawner;
    
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
        
        spawner.Spawn();
        
        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator GameStarting()
    {
        GameState = GameStates.Starting;
        Debug.Log("Current Game State: " + GameState);

        foreach (var player in players)
        {
            player.CanMove = true;
        }
        
        yield return null;
    }

    private IEnumerator GameRunning()
    {
        GameState = GameStates.Running;
        Debug.Log("Current Game State: " + GameState);

        while (AmountOfPlayersAlive() > 1)
        {
            Debug.Log("Running");
            yield return null;
        }
        
        yield return new WaitForSeconds(2.5f);
    }

    private IEnumerator GameFinishing()
    {
        GameState = GameStates.Finishing;
        Debug.Log("Current Game State: " + GameState);
        
        
        
        yield return null;
    }

    private int AmountOfPlayersAlive()
    {
        var playersAlive = players.Count;

        foreach (var _ in players.Where(player => !player.IsAlive))
            playersAlive--;

        return playersAlive;
    }
}
