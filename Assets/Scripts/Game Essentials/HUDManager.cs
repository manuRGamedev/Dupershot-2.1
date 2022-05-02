using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class HUDManager : MonoBehaviour
{
    [Header("Elementos UI")]
    [SerializeField] TMP_Text hitsText;
    [SerializeField] TMP_Text situationalText;
    [SerializeField] TMP_Text timerText;
    [SerializeField] TMP_Text timerCentsText;
    [Space]
    [Header("Estado del cañon")]
    [SerializeField] Image heatFill;
    [SerializeField] Image heatStackContainer;
    [SerializeField] Sprite[] heatStackStates;
    [SerializeField] int maxHeatStack = 10;

    [Header("Estado diégetico cañon")]
    [SerializeField] Gradient heatColorValues;
    [SerializeField] Image[] heatIndicators;

    [Header("Estado Dupershot")]
    [SerializeField] Image dupershotFill;

    [Header("Cuenta atrás")]
    [SerializeField] TMP_Text countDownText;

    [Header("Progreso dupershot")]
    [SerializeField] string[] dupershotPowerValues;
    [SerializeField] TMP_Text dupershotPowerValueText;
    [SerializeField] TMP_Text dupershotProgressText;

    PlayerController player;

    [Header("Elementos Combo")]
    [SerializeField] TMP_Text comboText;
    [SerializeField] Image comboFillLeft;
    [SerializeField] Image comboFillRight;
    [SerializeField] TMP_Text scoreText;

    [Header("LEFTIES")]

    [SerializeField] Image heatFillLEFTIES;
    [SerializeField] Image heatStackContainerLEFTIES;
    [SerializeField] Sprite[] heatStackStatesLEFTIES;
    [Space]
    [SerializeField] Image[] heatIndicatorsLEFTIES;
    [Space]
    [SerializeField] Image dupershotFillLEFTIES;

    [Header("Game Over")]
    [SerializeField] TMP_Text hitResultText;
    [SerializeField] TMP_Text hitHighscoreText;
    [SerializeField] TMP_Text comboResultText;
    [SerializeField] TMP_Text comboHighscoreText;
    [SerializeField] TMP_Text solidScoreText;
    [SerializeField] TMP_Text solidHighscoreText;

    // Start is called before the first frame update
    void Start()
    {
        //se obtiene la referencia del jugador
        player = PlayerController.instance;

        //Se inicializa la interfaz
        UpdateDiegeticHeatState(0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Medidor de calor
       heatFill.fillAmount = Mathf.Clamp(player.heatValue / 100, 0f, 1f);     
       heatFillLEFTIES.fillAmount = Mathf.Clamp(player.heatValue / 100, 0f, 1f);     
        heatStackContainer.sprite = GetStackState(player.heatStack);
        heatStackContainerLEFTIES.sprite = GetStackStateLEFTIES(player.heatStack);

        //Medidor meta de calor
        SetHeatColor(player.heatStack);
        UpdateDiegeticHeatState(player.heatValue);

        //Medidor Dupershot
        dupershotFill.fillAmount = Mathf.Clamp(player.dupershotValue / 100, 0f, 1f);
        dupershotFillLEFTIES.fillAmount = Mathf.Clamp(player.dupershotValue / 100, 0f, 1f);

        //Puntuación
        scoreText.text = ScoreManager.instance.solidScore.ToString();

        //Sistema de combo
        comboText.text = ScoreManager.instance.currentCombo.ToString();
        comboFillLeft.fillAmount = ScoreManager.instance.comboTimer / ScoreManager.instance.comboLimitTime;
        comboFillRight.fillAmount = ScoreManager.instance.comboTimer / ScoreManager.instance.comboLimitTime;

        //Cronometro
        float gameTime = ScoreManager.instance.gameTime;

        float min = gameTime / 60f;
        int sec = (int)(gameTime % 60f);

        System.TimeSpan ts =  System.TimeSpan.FromSeconds(gameTime);

        string currentTime = string.Format(@"{0:mm\:ss\.ff}",ts);

        //timerText.text = ((int)min).ToString() + ":" + sec.ToString("C2");
        timerText.text = currentTime;
        //timerCentsText.text = gameTime.ToString("");

        //Cuenta atrás tras la pausa
        if (GameManager.instance.state == GameState.Resuming)
        {
            UpdateCountDownText((int)GameManager.instance.countDownTimer);
        }
    }

    public void UpdatePowerupProgress(int objectiveValue, int objectiveScore, int lastObjectiveScore)
    {
        Debug.Log(ScoreManager.instance.solidScore);

        dupershotPowerValueText.text = dupershotPowerValues[objectiveValue];
        if (objectiveValue < dupershotPowerValues.Length - 1)
        {
            dupershotProgressText.text = "(" + ((int)(((float)(ScoreManager.instance.solidScore - lastObjectiveScore) / (objectiveScore - lastObjectiveScore))*100)).ToString() + "%)";
        }
        else
        {
            dupershotProgressText.text = "";
        }        
    }    

    public void UpdateDiegeticHeatState(float value)
    {
        int imageIndex = 0;

        do
        {
            Color tempColor = heatIndicators[imageIndex].color;

            if ((value - 60) > (40 / heatIndicators.Length))
            {
                
                tempColor = new Color(tempColor.r, tempColor.g, tempColor.b, 1f);
                value = Mathf.Clamp(value - (40 / heatIndicators.Length),0, 100f);
            }
            else
            {
                tempColor = new Color(tempColor.r, tempColor.g, tempColor.b, (value - 60) / 40);
            }

            heatIndicators[imageIndex].color = tempColor;

            imageIndex++;
        }
        while (imageIndex < heatIndicators.Length);
    }

    public void CheckHits()
    {
        if (ScoreManager.instance != null)
        {
            hitsText.text = ScoreManager.instance.hitScore.ToString();
        }
        else
        {
            hitsText.text = "0";
        }
        
    }

    public void SetHeatColor(float value)
    {
        foreach(Image temp in heatIndicators)
        {
            temp.color = heatColorValues.Evaluate(Mathf.Clamp(value, 0, maxHeatStack)/maxHeatStack);
        }
    }

    public Sprite GetStackState(float value)
    {
        if (value <= 1.5)
        {
            return heatStackStates[0];
        }
        else if (value <= 3)
        {
            return heatStackStates[1];
        }
        else if (value <= 5)
        {
            return heatStackStates[2];
        }
        else if (value <= 7.5)
        {
            return heatStackStates[3];
        }
        else
        {
            return heatStackStates[4];
        }       
    }

    public Sprite GetStackStateLEFTIES(float value)
    {
        if (value <= 1.5)
        {
            return heatStackStatesLEFTIES[0];
        }
        else if (value <= 3)
        {
            return heatStackStatesLEFTIES[1];
        }
        else if (value <= 5)
        {
            return heatStackStatesLEFTIES[2];
        }
        else if (value <= 7.5)
        {
            return heatStackStatesLEFTIES[3];
        }
        else
        {
            return heatStackStatesLEFTIES[4];
        }
    }

    public void UpdateCountDownText(int countDownValue)
    {
        countDownText.text = countDownValue.ToString();
    }

    public void SetGameOverStats()
    {
        hitResultText.text = ScoreManager.instance.hitScore.ToString();
        hitHighscoreText.text = ScoreManager.instance.hitHighscore.ToString();
        comboResultText.text = ScoreManager.instance.maxComboScore.ToString();
        comboHighscoreText.text = ScoreManager.instance.maxComboHighscore.ToString();
        solidScoreText.text = ScoreManager.instance.solidScore.ToString();
        solidHighscoreText.text = ScoreManager.instance.solidHighscore.ToString();
    }
}
