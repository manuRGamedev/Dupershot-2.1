using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowScores : MonoBehaviour
{
    [SerializeField] TMP_Text highscoreText;
    [SerializeField] TMP_Text maxComboText;
    [SerializeField] TMP_Text maxHitsText;
    [SerializeField] TMP_Text bestTimeText;
    [SerializeField] TMP_Text gamesPlayedText;
    [SerializeField] TMP_Text TotalDestroyedText;

    private void OnEnable()
    {
        DisplayScores();
    }

    void DisplayScores()
    {
        PlayerData current = SaveSystem.LoadPlayer();

        highscoreText.text = current.solidHighscore.ToString();
        maxComboText.text = current.maxCombo.ToString();
        maxHitsText.text = current.enemiesDestroyedHighscore.ToString();
        System.TimeSpan ts = System.TimeSpan.FromSeconds(current.bestTime);
        string bestTime = string.Format(@"{0:mm\:ss\.ff}", ts);
        bestTimeText.text = bestTime;
        gamesPlayedText.text = current.gamesPlayed.ToString();
    }
}
