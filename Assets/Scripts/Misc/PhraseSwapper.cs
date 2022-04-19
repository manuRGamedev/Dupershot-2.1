using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhraseSwapper : MonoBehaviour
{
    [SerializeField]TMP_Text phraseText;
    [SerializeField] float textDuration;
    float timer = 3f;
    [SerializeField]string[] phrases;
    int current = 0;

    // Start is called before the first frame update
    void Start()
    {
        current = Random.Range(0, phrases.Length);

        InvokeRepeating("ChangePhrase", 0f, textDuration);
    }

    void ChangePhrase()
    {
        current++;
        if (current >= phrases.Length) { current = 0; }
        phraseText.text = phrases[current] + "...";
    }

}
