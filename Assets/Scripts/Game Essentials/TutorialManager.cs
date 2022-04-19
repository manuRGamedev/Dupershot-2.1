using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] UnityEvent onTutorialTriggered;

    public void CheckOutTutorial()
    {
        if (!PlayerPrefs.HasKey("Tutorial"))
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            DisplayTutorial();
        }
    }

    public void DisplayTutorial()
    {
        onTutorialTriggered.Invoke();
    }
}
