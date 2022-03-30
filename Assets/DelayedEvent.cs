using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DelayedEvent : MonoBehaviour
{
    [SerializeField] float delay;
    [SerializeField] UnityEvent delayedEvent;

    public void InvokeEvent()
    {
        IEnumerator DelayInvoke()
        {
            yield return new WaitForSeconds(delay);
            delayedEvent.Invoke();
        }

        StartCoroutine(DelayInvoke());
    }
}
