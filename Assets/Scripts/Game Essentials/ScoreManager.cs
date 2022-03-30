using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    #region VARIABLES

    public static ScoreManager instance;

    [Header("Score Stats")]
    [SerializeField] int pointsPerHit;
    [SerializeField] int pointsPerDupershot;
    [SerializeField] int pointsPerSuperDupershot;

    //[HideInInspector]
    
    [HideInInspector] float fakeScore;
    [HideInInspector] public int HighScore;

    [Header("Combo System")]
    //[HideInInspector]
    public int currentCombo;
    public float comboLimitTime = 2.5f;
    //[HideInInspector]
    public float comboTimer = 1.5f;

    [Header("Combo Event")]
    [SerializeField] UnityEvent OnScoreAdded;

    [SerializeField] public List<GameObject> scoreIndicatorPool;

    #endregion
    public void SpawnSocreIndicator(int score, Vector3 impactPoint)
    {
        for (int i = 1; i < scoreIndicatorPool.Count - 1; i++)
        {
            if (scoreIndicatorPool[i].name == scoreIndicatorPool[0].name)
            {
                scoreIndicatorPool.RemoveAt(i);
                break;
            }
        }

        if (scoreIndicatorPool.Count > 0)
        {
            var newIndicator = scoreIndicatorPool[0];
            ScoreIndicator s = newIndicator.GetComponent<ScoreIndicator>();
            newIndicator.transform.position = impactPoint;
            newIndicator.SetActive(true);
            scoreIndicatorPool.Remove(newIndicator);
            s.SetScore(score);
        }
    }

    public void ReturnToPool(GameObject indicator)
    {
        scoreIndicatorPool.Add(indicator);

        indicator.SetActive(false);
    }

    #region SCORE TYPES

    //Important Data
    [HideInInspector] public float gameTime;
    [HideInInspector] public int hitScore;
    [HideInInspector] public int hitHighscore;
    [HideInInspector] public int maxComboScore;
    [HideInInspector] public int maxComboHighscore;
    [HideInInspector] public int solidScore;
    [HideInInspector] public int solidHighscore;
    [HideInInspector] public int duperShotsMade;
    [HideInInspector] public int maxComboWhenDupershot;

    #endregion

    #region MONOBEHAVIOUR METHODS

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        comboTimer = 0;
        currentCombo = 0;

        hitScore = 0;

        //scoreIndicatorPool = new List<GameObject>(25);
    }

    // Update is called once per frame
    void Update()
    {
        switch (GameManager.instance.state)
        {
            case GameState.Playing:
                ManageCombo();
                ManageTimer();


                break;
            default:
                break;
        }

    }

    #endregion        

    #region SCORE SAVING

    /// <summary>
    /// Reinicia la puntuación de la partida
    /// </summary>
    public void ResetScore()
    {
        comboTimer = 0;
        currentCombo = 0;

        gameTime = 0;

        hitScore = 0;
        hitHighscore = 0;
        maxComboScore = 0;
        maxComboHighscore = 0;
        fakeScore = 0f;
        solidScore = 0;
        solidHighscore = 0;
    }

    public void LoadHighscores()
    {
       hitHighscore = SaveSystem.LoadPlayer().enemiesDestroyedHighscore;
        maxComboHighscore = SaveSystem.LoadPlayer().maxCombo;
        solidHighscore = SaveSystem.LoadPlayer().solidHighscore;
        duperShotsMade = SaveSystem.LoadPlayer().DupershotsMade;
        maxComboWhenDupershot = SaveSystem.LoadPlayer().maxComboWhenDupershot;
    }

    public void SaveScores()
    {
        SaveSystem.SaveGame(this);
    }

    #endregion

    #region GAME SCORE MANAGING

    void AddScore(int points, Vector3 position)
    {
        //Manages the score
        float scoreToAdd = (1f + (float)currentCombo / 10f) * points;

        fakeScore += scoreToAdd;
        solidScore = Mathf.RoundToInt(fakeScore);
        
        if (solidScore > solidHighscore)
        {
            solidHighscore = solidScore;
        }

        //Sets active a score Indicator
        SpawnSocreIndicator(Mathf.RoundToInt(scoreToAdd), position);
    }

    void AddCombo()
    {
        currentCombo++;
        comboTimer = comboLimitTime;

        if (currentCombo > maxComboScore)
        {
            maxComboScore = currentCombo;

            if (maxComboScore > maxComboHighscore)
            {
                maxComboHighscore = maxComboScore;
            }
        }
    }

    void ManageCombo()
    {
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;

            if (comboTimer <= 0)
            {
                currentCombo = 0;
            }
        }
    }

    public void ReportHit(Vector3 position)
    {
        AddScore(pointsPerHit, position);
        AddCombo();

        hitScore++;

        if (hitScore > hitHighscore)
        {
            hitHighscore = hitScore;
        }
        OnScoreAdded.Invoke();
    }

    public void ReportSuperHit(Vector3 position)
    {
        AddScore(pointsPerDupershot, position);
        AddCombo();

        hitScore++;

        if (hitScore > hitHighscore)
        {
            hitHighscore = hitScore;
        }

        OnScoreAdded.Invoke();
    }

    public void ReportHit(int points, Vector3 position)
    {
        AddScore(points, position);
        AddCombo();

        hitScore++;

        if (hitScore > hitHighscore)
        {
            hitHighscore = hitScore;
        }
        OnScoreAdded.Invoke();
    }

    void ManageTimer()
    {
        gameTime += Time.deltaTime;
    }

    #endregion
}
