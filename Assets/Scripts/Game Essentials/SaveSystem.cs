using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
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
}


[System.Serializable]
public class PlayerData
{
    public int gamesPlayed;

    public int solidHighscore;
    public int maxCombo;

    public int enemiesDestroyedHighscore;
    public int enemiesDestroyedTotal;

    public int DupershotsMade;
    public int maxComboWhenDupershot;

    public float bestTime;

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
    public PlayerData()
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

