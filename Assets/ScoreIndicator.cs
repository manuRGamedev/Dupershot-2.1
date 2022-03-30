using System;
using UnityEngine;
using TMPro;

public class ScoreIndicator : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] float lifetime;
    float timer = 0;

    [Header("Customization")]
    [SerializeField] ScoreColor[] scoreColors;


    [Serializable]
    public struct ScoreColor
    {
        public int maxScore;
        public Color scoreColor;
    }

    private void OnEnable()
    {
        timer = 0;
        Debug.Log("SCORE INDICATOR APPEARS");
    }

    /// <summary>
    /// Asigna un valor al texto y un color en función de lo alto que sea el valor
    /// </summary>
    /// <param name="value"></param>
    public void SetScore(int value)
    {
        //Se cambia el texto
        scoreText.text = value.ToString();

        //Si está dentro de determinado rango, adopta un nuevo color
        for (int i = 0; i < scoreColors.Length - 1; i++)
        {
            if (value < scoreColors[i].maxScore)
            {
                scoreText.color = scoreColors[i].scoreColor;
                return;
            }
        }
        //Si no se ha determinado ningún color se escoge el último
        scoreText.color = scoreColors[scoreColors.Length - 1].scoreColor;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if ( timer >= lifetime ) {Debug.Log("INDICATOR RETOURNED TO POOL"); ScoreManager.instance.ReturnToPool(gameObject); }
    }
}
