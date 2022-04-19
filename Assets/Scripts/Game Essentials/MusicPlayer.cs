using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioClip musicClip;
    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = PlayerPrefs.GetFloat("MusicVolume") * PlayerPrefs.GetFloat("MasterVolume");
        source.clip = musicClip;
        source.Play();
    }
}
