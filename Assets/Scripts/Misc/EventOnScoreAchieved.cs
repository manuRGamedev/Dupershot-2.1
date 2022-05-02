using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnScoreAchieved : MonoBehaviour
{
    [SerializeField] UnityEvent OnScoreReached;
    [SerializeField] int[] objectives;
    [SerializeField] int objectiveValue;

    [SerializeField] HUDManager hud;
    ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = ScoreManager.instance;
    }

    public void ResetObjective()
    {
        objectiveValue = 0;
        scoreManager.ResetScore();
        hud.UpdatePowerupProgress(objectiveValue, objectives[objectiveValue], (objectiveValue - 1) < 0 ? 0 : objectives[objectiveValue - 1]);
    }

    public void CheckObjective()
    {
        if (objectiveValue == objectives.Length) return;

        if (scoreManager.solidScore > objectives[objectiveValue])
        {
            objectiveValue++;
            OnScoreReached.Invoke();
        }

        hud.UpdatePowerupProgress(objectiveValue, objectives[objectiveValue < objectives.Length? objectiveValue : objectiveValue - 1], (objectiveValue - 1) < 0 ? 0 : objectives[objectiveValue - 1]);
    }

}
