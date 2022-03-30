using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CallAudioManagerProperty : MonoBehaviour
{
    [InspectorName("Play on enable?")] public bool playOnEnable = false;
    [InspectorName("Sounds to Call")]public CallSound[] soundsToCall;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            PlayDelayedList();
        }
    }

    public void PlayDelayedList()
    {
        foreach (CallSound c in soundsToCall)
        {
            StartCoroutine(PlayWithDelay(c.soundName, c.delay));
        }

        IEnumerator PlayWithDelay(string soundName, float delay)
        {
            yield return new WaitForSeconds(delay);
            Play(soundName);
        }
    }

    public void Play(string name)
    {
        AudioManager.instance.Play(name);
    }

    public void PlayAtPitch(string name, float pitch)
    {
        AudioManager.instance.PlayAtPitch(name, pitch);
    }
    [System.Serializable]
    public struct CallSound
    {
        public string soundName;
        public float delay;
    }
}
/*

[CustomEditor(typeof(CallAudioManagerProperty))]
public class ShowHideToggle : Editor
{
    override public void OnInspectorGUI()
    {
        var thisScript = target as CallAudioManagerProperty;

        thisScript.playOnEnable = GUILayout.Toggle(thisScript.playOnEnable, "PlayOnEnable");
        
        
        if (thisScript.playOnEnable)
        {
            thisScript.soundsToCall = EditorGUILayout.TextField("Which sound?", thisScript.soundName);
        }
    }
}
*/
