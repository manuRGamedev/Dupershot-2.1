using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    /* SISTEMA ANTIGUO
    public static void SaveGame(ScoreManager score)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "player.scores";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(score);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SaveNewGame()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "player.scores";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void EraseData()
    {
        string path = Application.persistentDataPath + "player.scores";

        if (!File.Exists(path))
        {
            SaveNewGame();
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        PlayerData data = new PlayerData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "player.scores";

        if (!File.Exists(path))
        {
            SaveNewGame();
        }

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Open);

        PlayerData data = formatter.Deserialize(stream) as PlayerData;
        stream.Close();

        return data;
    }
*/

    public static void SaveGame(ScoreManager score)
    {
        PlayerData data = new PlayerData(score);
    }

    public static void SaveNewGame()
    {
        PlayerData data = new PlayerData(false);
    }

    public static void EraseData()
    {
        PlayerData data = new PlayerData(false);
    }

    public static PlayerData LoadPlayer()
    {
        PlayerData data = new PlayerData(true);
        return data;
    }
}

[System.Serializable]
public class PlayerData
{
    public int gamesPlayed
    {
        get
        {
            return PlayerPrefs.GetInt("Scores_GamesPlayed");
        }
        set
        {
            PlayerPrefs.SetInt("Scores_GamesPlayed", value);
        }
    } 

    public int solidHighscore
    {
        get
        {
            return PlayerPrefs.GetInt("Scores_SolidHighscore");
        }
        set
        {
            PlayerPrefs.SetInt("Scores_SolidHighscore", value);
        }
    }
    public int maxCombo 
    {
        get
        {
            return PlayerPrefs.GetInt("Scores_MaxCombo");
        }
        set
        {
            PlayerPrefs.SetInt("Scores_MaxCombo", value);
        }
    }

    public int enemiesDestroyedHighscore
    {
        get
        {
            return PlayerPrefs.GetInt("Scores_EnemiesDestroyedHighscore");
        }
        set
        {
            PlayerPrefs.SetInt("Scores_EnemiesDestroyedHighscore", value);
        }
    }
    public int enemiesDestroyedTotal
    {
        get
        {
            return PlayerPrefs.GetInt("Scores_EnemiesDestroyedTotal");
        }
        set
        {
            PlayerPrefs.SetInt("Scores_EnemiesDestroyedTotal", value);
        }
    }

    public int DupershotsMade;
    public int maxComboWhenDupershot;

    public float bestTime
    {
        get
        {
            return PlayerPrefs.GetFloat("Scores_BestTime");
        }
        set
        {
            PlayerPrefs.SetFloat("Scores_BestTime", value);
        }
    }

    private int Compare(int oldValue, int newValue)
    {
        if (oldValue >= newValue)
        {
            return oldValue;
        }
        else
        {
            return newValue;
        }
    }

    private float CompareFloat(float oldValue, float newValue)
    {
        if (oldValue >= newValue)
        {
            return oldValue;
        }
        else
        {
            return newValue;
        }
    }


    /// <summary>
    /// Constructor que se emplea cuando no hay ningun dato.
    /// </summary>
    public void EraseData()
    {
        gamesPlayed = 0;

        solidHighscore = 0;
        maxCombo = 0;

        enemiesDestroyedHighscore = 0;
        enemiesDestroyedTotal = 0;

        DupershotsMade = 0;
        maxComboWhenDupershot = 0;

        bestTime = 0f;
    }

    public PlayerData(bool value)
    {
        if (value)
        {
            gamesPlayed = PlayerPrefs.GetInt("Scores_GamesPlayed");

            solidHighscore = PlayerPrefs.GetInt("Scores_SolidHighscore");
            maxCombo = PlayerPrefs.GetInt("Scores_MaxCombo");

            enemiesDestroyedHighscore = PlayerPrefs.GetInt("Scores_EnemiesDestroyedHighscore");
            enemiesDestroyedTotal = PlayerPrefs.GetInt("Scores_EnemiesDestroyedTotal");

            DupershotsMade = 0;
            maxComboWhenDupershot = 0;

            bestTime = PlayerPrefs.GetFloat("Scores_BestTime");
        }
        else
        {
            gamesPlayed = 0;

            solidHighscore = 0;
            maxCombo = 0;

            enemiesDestroyedHighscore = 0;
            enemiesDestroyedTotal = 0;

            DupershotsMade = 0;
            maxComboWhenDupershot = 0;

            bestTime = 0f;
        }
    }


    public PlayerData(ScoreManager score)
    {
        gamesPlayed = Compare(gamesPlayed, gamesPlayed + 1) ;

        solidHighscore = Compare(solidHighscore, score.solidHighscore);
        maxCombo = Compare(maxCombo, score.maxComboHighscore);

        enemiesDestroyedHighscore = Compare(enemiesDestroyedHighscore, score.hitHighscore);
        enemiesDestroyedTotal = Compare(enemiesDestroyedTotal ,enemiesDestroyedTotal + score.hitScore);

        DupershotsMade += score.duperShotsMade;
        maxComboWhenDupershot= Compare(maxComboWhenDupershot, score.maxComboWhenDupershot);

        bestTime = CompareFloat(bestTime, score.gameTime);
    }
}

