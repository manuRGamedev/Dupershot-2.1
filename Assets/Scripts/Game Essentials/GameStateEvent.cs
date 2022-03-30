using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameStateEvent : MonoBehaviour
{
    [SerializeField] bool fromPool = false;
    [Header("Core States")]
    [SerializeField] UnityEvent OnGameSetMenu;
    [SerializeField] UnityEvent OnGameSetStart;
    [Space]
    [Header("In-Game States")]
    [SerializeField] UnityEvent OnGameSetPlaying;
    [SerializeField] UnityEvent OnGameSetFrozen;
    [SerializeField] UnityEvent OnGameSetPaused;
    [SerializeField] UnityEvent OnGameSetResuming;
    [SerializeField] UnityEvent OnGameSetResurrecting;
    [SerializeField] UnityEvent OnGameSetGameOver;

    // Start is called before the first frame update
    void Awake()
    {
        GameManager.OnGameStateChanged += ExecuteGameStateEvent;
    }    

    private void OnEnable()
    {
        if (fromPool)
        {
            ExecuteGameStateEvent(GameManager.instance.state);
        }
    }

    public void ExecuteGameStateEvent(GameState newState)
    {
        switch (newState)
        {
            case GameState.Menu:
                OnGameSetMenu.Invoke();
                break;
            case GameState.Start:
                OnGameSetStart.Invoke();
                break;
            case GameState.Playing:
                OnGameSetPlaying.Invoke();
                break;
            case GameState.Frozen:
                OnGameSetFrozen.Invoke();
                break;
            case GameState.Paused:
                OnGameSetPaused.Invoke();
                break;
            case GameState.Resuming:
                OnGameSetResuming.Invoke();
                break;
            case GameState.Resurrecting:
                OnGameSetResurrecting.Invoke();
                break;
            case GameState.GameOver:
                OnGameSetGameOver.Invoke();
                break;
        }
    }
}