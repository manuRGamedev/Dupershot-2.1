using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyData", menuName = "EnemyData")]
[System.Serializable]
public class EnemyData : ScriptableObject
{
    [Header("Main Properties")]
    [SerializeField] string name = "New Threat";
    public bool isAsteroid = true;
    [SerializeField] GameObject enemyPrefab;
    public GameObject particlesPrefab;
    [HideInInspector] public float probabilityValue;
    public int numberOfInstances = 10;
    public int spaceInPool = 1;

    [HideInInspector]public Transform enemyContainer;

    [Header("Probability Management Properties")]
    [SerializeField][Range(-100f,100f)] float probabilityStartValue = 10f;
    [SerializeField] float probabilityIncreaseValue = 10f;
    [SerializeField] float maxProbabilityIncreaseValue = 20f;
    float auxProbabilityIncreaseValue = 10f;
    [SerializeField] float probabilityDescreaseValue= 50f;

    [Header("Restrictions")]
    [SerializeField] int restrictedRounds;
    int restrictionValue;

    [Header("Level Up Management Properties")]
    [SerializeField] [Range(0f, 100f)] float probabilityIncreaseLevelUpValue;

    //METHODS

    public void Initialize()
    {
        probabilityValue = probabilityStartValue;
        auxProbabilityIncreaseValue = probabilityIncreaseValue;

        restrictionValue = restrictedRounds;
    }

    public void AssignContainer(Transform newContrainer)
    {
        enemyContainer = newContrainer;
    }

    public void UpdateProbability()
    {
        if (restrictionValue <= 0) probabilityValue = Mathf.Clamp(probabilityValue + auxProbabilityIncreaseValue, 0, 100);
    }

    public void ReduceProbability()
    {
        probabilityValue -= probabilityDescreaseValue;
    }

    public void LevelUpEnemy()
    {
        if (restrictionValue > 0)
        {
            restrictionValue--;
            return;
        }
        else
        {
            auxProbabilityIncreaseValue = Mathf.Clamp(auxProbabilityIncreaseValue + probabilityIncreaseLevelUpValue, 0, maxProbabilityIncreaseValue);
        }        
    }

    public GameObject GetEnemyPrefab()
    {
        return enemyPrefab;
    }
}
