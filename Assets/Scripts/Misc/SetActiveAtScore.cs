using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveAtScore : MonoBehaviour
{
    [SerializeField] Upgrade[] upgrades;

    public void CheckUpgrades()
    {
        int score = ScoreManager.instance.solidScore;

        foreach (Upgrade upgrade in upgrades)
        {
            if (score >= upgrade.scoreGoal)
            {
                upgrade.item.SetActive(true);
            }
            else
            {
                upgrade.item.SetActive(false);
            }
        }
    }

    [System.Serializable]
    struct Upgrade
    {
        public GameObject item;
        public int scoreGoal;
    }
}
