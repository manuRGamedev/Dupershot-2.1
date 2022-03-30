using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShakeController : MonoBehaviour
{
    public static CamShakeController instance;
    Vector3 startPosition;

    [Header("CamShake variables")]
    [SerializeField] float camShakeIntensity = 0.15f;
    [SerializeField] int camShakeAmount = 6;
    [SerializeField] float camShakeDuration = 0.08f;

    [Header("ShotShake variables")]
    [SerializeField] float shotShakeIntensity = 0.1f;
    [SerializeField] int shotShakeAmount = 4;
    [SerializeField] float shotShakeDuration = 0.05f;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ShotShake(int state)
    {
        bool back = true;
        Vector3 recoilPosition = Vector3.zero;

        switch (state)
        {
            case 1:
                recoilPosition = new Vector3(1, 0, -10);
                break;

            case 2:
                recoilPosition = new Vector3(0, -1, -10);
                break;

            case 3:
                recoilPosition = new Vector3(-1, 0, -10);
                break;

            case 4:
                recoilPosition = new Vector3(0, 1, -10);
                break;
        }

        recoilPosition = Vector3.Scale(recoilPosition, new Vector3(shotShakeIntensity, shotShakeIntensity, 1)); 
        for (int i = 0; i < shotShakeAmount; i++)
        {
            if (back) transform.position = recoilPosition;
            else transform.position = startPosition;
            back = !back;
            yield return new WaitForSeconds(shotShakeDuration);
        }
        transform.position = startPosition;
    }

    public IEnumerator CamShake()
    {
        int r = Random.Range(1, 3);
        bool back = true;
        Vector3 shakePosition = Vector3.zero;

        switch (r)
        {
            case 1:
                shakePosition = new Vector3(1, 0, -10);
                break;

            case 2:
                shakePosition = new Vector3(0, 1, -10);
                break;            
        }

        shakePosition = Vector3.Scale(shakePosition, new Vector3(camShakeIntensity, camShakeIntensity, 1));
        for (int i = 0; i < camShakeAmount; i++)
        {
            shakePosition = Vector3.Scale(shakePosition, new Vector3(-1, -1, 1));

            float time = 0;
            Vector3 actualPosition = transform.position;
            while(time < camShakeDuration)
            {
                transform.position = Vector3.Lerp(actualPosition, shakePosition, time/camShakeDuration);
                time += Time.fixedDeltaTime;
                yield return null;
            }
            transform.position = shakePosition;
            back = !back;
            yield return new WaitForSeconds(shotShakeDuration);
        }
        transform.position = startPosition;
    }
}
