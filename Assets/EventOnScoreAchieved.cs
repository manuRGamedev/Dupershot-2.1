using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnScoreAchieved : MonoBehaviour
{
    [SerializeField] UnityEvent OnScoreReached;
    int[] objectives;
    int objectiveValue;
    ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = ScoreManager.instance;
    }

    public void ResetObjective()
    {
        objectiveValue = 0;
    }

    public void CheckObjective()
    {
        if (scoreManager.solidScore > objectives[objectiveValue])
        {
            objectiveValue++;
            OnScoreReached.Invoke();
        }
    }

}
