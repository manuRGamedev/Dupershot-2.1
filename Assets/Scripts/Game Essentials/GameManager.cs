using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region VARIABLES

    public static GameManager instance;
    public GameState state { get; private set; }

    [Header("Progresión")]
    public int startDificultyLevel = 1;
    [SerializeField][InspectorName("Progression Multiplier")] float progressionConstant = 1.75f;
    int currentLevel;
    [SerializeField] int firstLevelUpRequirement = 9;
    int nextLevelTarget;
    [SerializeField] float levelUpTime = 10f;
    float levelUpTimer;
    [HideInInspector]public int destroyedEnemies;


    [Header("Sistema de vidas")]
    public int playerLives = 2;
    [SerializeField] float resurrectionTime = 0.5f;

    [Header("Pausa")]
    [HideInInspector] public float countDownTimer;
    [SerializeField] float countDownTime = 3f;

    [Header("Variables de Game Over")]
    [SerializeField] float deadTime = 1f;

    public static event Action<GameState> OnGameStateChanged;

    #endregion
    [SerializeField] bool debug = true;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateGameState(GameState.Menu);
    }    

    #region PROGRESSION

    void InitializeProgresionValues()
    {
        currentLevel = 0;
        levelUpTimer = 0;
        nextLevelTarget = firstLevelUpRequirement;
    }

    public void LevelUp()
    {
        if (debug) { Debug.Log("LEVEL UP to " + currentLevel.ToString()); }
        currentLevel ++;
        EnemyPool.instance.LevelUpEnemies(currentLevel);

        //Se calcula el target siguiente
        int tempLevel = currentLevel + 1;

        nextLevelTarget = (int)(nextLevelTarget * progressionConstant);
    }

    public bool CanLevelUp()
    {
        if (destroyedEnemies >= nextLevelTarget)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddEnemyDestroyed()
    {
        destroyedEnemies++;
        /*
        if (CanLevelUp())
        {
            LevelUp();
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (state == GameState.Playing)
        {
            levelUpTimer += Time.deltaTime;
            if (levelUpTimer >= levelUpTime)
            {
                levelUpTimer = 0;
                LevelUp();
            }
        }
    }    

    #endregion

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Menu:
                break;
            case GameState.Start:
                InitializeProgresionValues();
                break;
            case GameState.Playing:
                break;
            case GameState.Frozen:
                break;
            case GameState.Paused:
                break;
            case GameState.Resuming:
                break;
            case GameState.Resurrecting:
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, "The term " + newState.ToString() + " is not a Game State.");
        }

        OnGameStateChanged?.Invoke(newState); 
    }

    #region GAME STATE METHODS

    public void SetMenu()
    {
        UpdateGameState(GameState.Menu);
    }

    public void SetStart()
    {
        IEnumerator StartGameCorroutine()
        {
            float startTime = 2f;
            UpdateGameState(GameState.Start);

            do
            {
                startTime -= Time.deltaTime;
                yield return null;
            }
            while (startTime > 0);

            UpdateGameState(GameState.Playing);
        }

        StartCoroutine(StartGameCorroutine());

        
    }

    public void SetPlaying()
    {
        UpdateGameState(GameState.Playing);
    }

    public void SetPause()
    {
        UpdateGameState(GameState.Paused);
    }

    public void SetResurrection()
    {
        UpdateGameState(GameState.Resurrecting);
    }

    public void SetResuming()
    {
        IEnumerator ResumeCorroutine()
        {
            countDownTimer = countDownTime;
            UpdateGameState(GameState.Resuming);

            do
            {
                countDownTimer -= Time.deltaTime;
                yield return null;
            }
            while (countDownTimer > 1);

            UpdateGameState(GameState.Playing);
        }

        StartCoroutine(ResumeCorroutine());
    }

    /*
    private void OnApplicationPause(bool pause)
    {
        StopAllCoroutines();
        UpdateGameState(GameState.Paused);
    }
    */

    /// <summary>
    /// Congela la partida durante cierto periodo de tiempo
    /// </summary>
    /// <param name="freezeTime"></param>
    public void FreezeByTime(float freezeTime)
    {
        IEnumerator FreezeCorroutine(float time)
        {
            UpdateGameState(GameState.Frozen);

            yield return new WaitForSeconds(time);

            UpdateGameState(GameState.Playing);
        }

        StartCoroutine(FreezeCorroutine(freezeTime));
    }

    public void CheckResurrection()
    {
        IEnumerator AttemtResurrection()
        {
            yield return new WaitForSeconds(deadTime);

            if (playerLives > 0)
            {
                playerLives--;
                UpdateGameState(GameState.Resurrecting);
                yield return null;
                FreezeByTime(resurrectionTime);

                float time = 0;
                while (time < resurrectionTime)
                {
                    time += Time.deltaTime;
                    yield return null;
                }
                UpdateGameState(GameState.Playing);                
            }
            else
            {
                UpdateGameState(GameState.GameOver);                
            }
        }

        StartCoroutine(AttemtResurrection());
    }

    #endregion
}
public enum GameState { Menu, Start, Playing, Frozen, Paused, Resuming, Resurrecting, GameOver }