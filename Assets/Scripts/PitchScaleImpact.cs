using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchScaleImpact : MonoBehaviour
{
    int impactValue;
    [SerializeField] int maxImpactValue;
    [SerializeField] float minPitch = 0.8f;
    [SerializeField] float maxPitch = 1.6f;
    [SerializeField] string soundToCall;

    private void OnEnable()
    {
        impactValue = 0;
    }

    public void StackImpact()
    {
        float pitch = minPitch + maxPitch * Mathf.Min(impactValue, maxImpactValue);

        impactValue++;
        AudioManager.instance.PlayAtPitch(soundToCall, pitch);
    }
}
