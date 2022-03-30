using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSettings : MonoBehaviour
{
    [SerializeField] GameObject rightUI;
    [SerializeField] GameObject leftUI;

    public void SetUI()
    {
        switch (PlayerPrefs.GetInt("IsRightHanded"))
        {
            case 0:
                rightUI.SetActive(true);
                leftUI.SetActive(false);
                break;

            case 1:
                rightUI.SetActive(false);
                leftUI.SetActive(true);
                break;
        }
    }
}
