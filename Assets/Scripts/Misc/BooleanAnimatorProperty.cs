using UnityEngine;

public class BooleanAnimatorProperty : MonoBehaviour
{
    [SerializeField] string booleanID = "new Boolean";
    [SerializeField] bool startValue = true;

    private void Start()
    {
        if (GetComponent<Animator>())
        {
            GetComponent<Animator>().SetBool(booleanID, startValue);
        }
        else
        {
            Debug.LogError("The Object " + this.gameObject.name + "does't contain an animator, " +
                "but it is trying to access it. Add an animator component or remove the BooleanAnimatorPropertyComponent.");
        }
    }

    public void SetAnimatorBoolean(bool value)
    {
        if (GetComponent<Animator>())
        {
            GetComponent<Animator>().SetBool(booleanID, value);
        }
        else
        {
            Debug.LogError("The Object " + this.gameObject.name + "does't contain an animator, " +
                "but it is trying to access it. Add an animator component or remove the BooleanAnimatorPropertyComponent.");
        }
    }
}
