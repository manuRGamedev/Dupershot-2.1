using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] GameObject tapObject;
    [SerializeField] GameObject questionObject;
    [SerializeField] GameObject verificationObject;
    [SerializeField] GameObject loadingObject;
    [Space]
    [SerializeField] Image loadingBar;


    // Start is called before the first frame update
    void Awake()
    {
        //Setea el Master Volume
        if (!PlayerPrefs.HasKey("MasterVolume"))
        {
            PlayerPrefs.SetFloat("MasterVolume", 1f);
        }

        //Setea el Music Volume
        if (!PlayerPrefs.HasKey("MusicVolume"))
        {
            PlayerPrefs.SetFloat("MusicVolume", 1f);
        }

        //Setea el Fx Volume
        if (!PlayerPrefs.HasKey("FxVolume"))
        {
            PlayerPrefs.SetFloat("FxVolume", 1f);
        }

        //Screen Elements
        tapObject.SetActive(true);
        questionObject.SetActive(false);
        verificationObject.SetActive(false);
        loadingObject.SetActive(false);

    } 

    public void CheckForPlayerPrefs()
    {
        tapObject.SetActive(false);

        if (!PlayerPrefs.HasKey("IsRightHanded"))
        {
            questionObject.SetActive(true);
        }
        else
        {
            loadingObject.SetActive(true);
        }
    }

    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    public void LoadGameScreen()
    {
        StartCoroutine(LoadAsynchonously());

        IEnumerator LoadAsynchonously()
        {           

            float time = 0f;
            float value = 0f;

            while(time < 2f)
            {
                value += 1f * Time.deltaTime/ 2f;
                loadingBar.fillAmount = value;
                time += Time.deltaTime;
                yield return null;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync("GameScene");
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                //float progress = Mathf.Clamp01((operation.progress/28.6f + 0.65f) / .9f);

                //loadingBar.fillAmount = progress;
                if (operation.progress > 0.89f)
                {
                    operation.allowSceneActivation = true;
                }

                yield return new WaitForEndOfFrame();
            }

            loadingObject.SetActive(false);
        }
    }

    public void SetHand(int value)
    {
        switch (value)
        {
            case 0:
                //Setea el valor de los controles
                PlayerPrefs.SetInt("IsRightHanded", 0);
                break;

            case 1:
                //Setea el valor de los controles
                PlayerPrefs.SetInt("IsRightHanded", 1);
                break;

            default:
                break;
        }
    }    
}
