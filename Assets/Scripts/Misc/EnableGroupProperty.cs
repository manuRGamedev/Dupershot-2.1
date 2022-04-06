using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGroupProperty : MonoBehaviour
{
    [Tooltip("Grupo de objetos que estará habilidado al comienzo. En caso de usar un valor menor que cero, no se hablilitará ninguno")]
    [SerializeField] int firstEnabledGroup = -1;
    int current = 0;
    [SerializeField] ObjectGroup[] objectGroups;

    // Start is called before the first frame update
    void Start()
    {
        current = 0;

        SetGroupEnabled(firstEnabledGroup);
    }

    private void OnEnable()
    {
        SetGroupEnabled(firstEnabledGroup);
    }

    public void SetGroupEnabled(int value)
    {
        ObjectGroup selected = null;

        if (value >= objectGroups.Length) { value = 0; }
        else if (value < 0) { value = objectGroups.Length - 1; }

        current = value;

        for (int i = 0; i < objectGroups.Length; i++)
        {
            if (i != value)
            {
                foreach (GameObject g in objectGroups[i].objects)
                {
                    g.SetActive(false);
                }                
            }
            else
            {
                selected = objectGroups[i];
            }
        }

        if (selected != null)
        {
            foreach (GameObject g in selected.objects)
            {
                g.SetActive(true);
            }
        }
    }

    public void SetNext()
    {
        SetGroupEnabled(current + 1);
    }

    public void SetPrevious()
    {
        SetGroupEnabled(current - 1);
    }
}

[System.Serializable]
public class ObjectGroup
{
    [HideInInspector] public string name = "Group";
    public GameObject[] objects;
}
