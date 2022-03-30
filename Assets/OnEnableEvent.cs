using UnityEngine;
using UnityEngine.Events;

public class OnEnableEvent : MonoBehaviour
{
    [SerializeField] UnityEvent OnGameObjectSetActive;

    private void OnEnable()
    {
        OnGameObjectSetActive.Invoke();
    }
}
