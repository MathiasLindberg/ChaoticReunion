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
    [SerializeField] private List<Player> players = new();
    [SerializeField] private Spawner spawner;

    [Header("Debug Settings")] 
    [SerializeField] private bool ignoreInsufficientAmountOfControllers;
    
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

        UIViewManager.Instance.EnableUIViewExclusive(GameState);

        if (!ignoreInsufficientAmountOfControllers)
        {
            while (AmountOfControllersConnected() != players.Count)
            {
                yield return null;
            }
        }
        
        spawner.Spawn();

        yield return null;
    }

    private IEnumerator GameStarting()
    {
        GameState = GameStates.Starting;

        UIViewManager.Instance.EnableUIViewExclusive(GameState);
        
        while (AmountOfPlayersShook() < 1)
        {
            yield return null;
        }

        // Ugly ...
        ((GameStartingView) UIViewManager.Instance.CurrentUIView).shakeToPlay.SetActive(false);
        ((GameStartingView) UIViewManager.Instance.CurrentUIView).gameCountdown.SetActive(true);
        yield return new WaitForSeconds(3.25f);
        ((GameStartingView) UIViewManager.Instance.CurrentUIView).gameCountdown.SetActive(false);
        
        foreach (var player in players)
        {
            player.CanMove = true;
        }
        
        yield return null;
    }

    private IEnumerator GameRunning()
    {
        GameState = GameStates.Running;
        
        UIViewManager.Instance.EnableUIViewExclusive(GameState);

        while (AmountOfPlayersAlive() > 1)
        {
            yield return null;
        }

        yield return null;
    }

    private IEnumerator GameFinishing()
    {
        GameState = GameStates.Finishing;

        UIViewManager.Instance.EnableUIViewExclusive(GameState);

        foreach (var player in players)
            player.CanMove = false;

        yield return null;
    }

    private int AmountOfControllersConnected()
    {
        return players.Count(player => player.isSensorConnected);
    }

    private int AmountOfPlayersShook()
    {
        return players.Count(player => player.HasShaken);
    }
    
    private int AmountOfPlayersAlive()
    {
        var playersAlive = players.Count;

        foreach (var _ in players.Where(player => !player.IsAlive))
            playersAlive--;

        return playersAlive;
    }
}
