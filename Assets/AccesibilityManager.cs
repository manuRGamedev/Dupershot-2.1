using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AccesibilityManager : MonoBehaviour
{
    [SerializeField]
    bool isRight;

    [SerializeField] Button leftiesButton;
    [SerializeField] Button rightiesButton;
    [Space]
    [SerializeField] Sprite leftDisabled;
    [SerializeField] Sprite leftEnabled;
    [SerializeField] Sprite rightDisabled;
    [SerializeField] Sprite rightEnabled;
    [Space]
    [SerializeField] UnityEvent OnRightiesEnabled;
    [SerializeField] UnityEvent OnLeftiesEnabled;
    [Space]
    [SerializeField] EventSystem mainEventSystem; 
   
    /// <summary>
    /// Establece el valor de la mano en uso ("0" para diestro, "1" para zurdo").
    /// </summary>
    /// <param name="value">Valor que determina si establecemos que el jugador es diestro(0) o zurdo(1).</param>
    public void SetHandValue(int value)
    {
        PlayerPrefs.SetInt("IsRight", value);

        switch (value)
        {
            case 0:
                isRight = true;
                leftiesButton.image.sprite = leftDisabled;                
                rightiesButton.image.sprite = rightEnabled;                
                break;

            case 1:
                isRight = false;
                leftiesButton.image.sprite = leftEnabled;                
                rightiesButton.image.sprite = rightDisabled;                
                break;

            default:               
                break;
        }

        mainEventSystem.SetSelectedGameObject(null);
    }
}
