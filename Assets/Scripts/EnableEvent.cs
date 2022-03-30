using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnableEvent : MonoBehaviour
{
    [SerializeField] ComponentEvent[] ComponentEvents = new ComponentEvent[1];

    [System.Serializable]
    struct ComponentEvent
    {
        public float delay;
        public UnityEvent OnComponentSetEnabled;

        
    }

    private void OnEnable()
    {        
        foreach (ComponentEvent c in ComponentEvents)
        {
            StartCoroutine(DelayedEventCOrroutine(c));
        }
    }

    private IEnumerator DelayedEventCOrroutine(ComponentEvent c)
    {
        yield return new WaitForSeconds(c.delay);

        c.OnComponentSetEnabled.Invoke();
    }
}
