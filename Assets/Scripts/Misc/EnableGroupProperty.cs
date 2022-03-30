using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGroupProperty : MonoBehaviour
{
    [Tooltip("Grupo de objetos que estará habilidado al comienzo. En caso de usar un valor menor que cero, no se hablilitará ninguno")]
    [SerializeField] int firstEnabledGroup = -1;
    [SerializeField] ObjectGroup[] objectGroups;

    // Start is called before the first frame update
    void Start()
    {
        SetGroupEnabled(firstEnabledGroup);
    }
    public void SetGroupEnabled(int value)
    {
        for (int i = 0; i < objectGroups.Length; i++)
        {
            if (i == value)
            {
                foreach (GameObject g in objectGroups[i].objects)
                {
                    g.SetActive(true);
                }                
            }
            else
            {
                foreach (GameObject g in objectGroups[i].objects)
                {
                    g.SetActive(false);
                }
            }
        }
    }
}

[System.Serializable]
public class ObjectGroup
{
    [HideInInspector] public string name = "Group";
    public GameObject[] objects;
}
