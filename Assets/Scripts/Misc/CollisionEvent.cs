using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    [Header("Conditions")]
    [SerializeField] bool isTrigger = false;

    [SerializeField] string[] triggerTags;
    [SerializeField] UnityEvent OnColliderEnter;
    [SerializeField] UnityEvent OnColliderExit;

    /// <summary>
    /// Checks if the collider tag equals to any of the array "triggerTags".
    /// </summary>
    /// <returns></returns>
    private bool TagMatch(string tag)
    {
        foreach (string t in triggerTags)
        {
            if (t == tag)
            {
                return true;
            }
        }

        return false;
    }

    #region 3D COLLISIONS

    private void OnTriggerEnter(Collider other)
    {
        if (TagMatch(other.tag) && isTrigger)
        {
            OnColliderEnter.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (TagMatch(collision.gameObject.tag) && !isTrigger)
        {
            OnColliderEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (TagMatch(other.tag) && isTrigger)
        {
            OnColliderExit.Invoke();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (TagMatch(collision.gameObject.tag) && !isTrigger)
        {
            OnColliderExit.Invoke();
        }
    }

    #endregion

    #region 2D COLLISIONS

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TagMatch(collision.tag) && isTrigger)
        {
            OnColliderEnter.Invoke();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (TagMatch(collision.gameObject.tag) && !isTrigger)
        {
            OnColliderEnter.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (TagMatch(collision.tag) && isTrigger)
        {
            OnColliderExit.Invoke();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (TagMatch(collision.gameObject.tag) && !isTrigger)
        {
            OnColliderExit.Invoke();
        }
    }

    #endregion
}
