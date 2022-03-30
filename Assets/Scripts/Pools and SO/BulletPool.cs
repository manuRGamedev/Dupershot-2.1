using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool instance;
    [SerializeField] public List<GameObject> bulletPool;
    [SerializeField] GameObject dupershot;    
    Bullet bulletController;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {

            instance = this;
            DontDestroyOnLoad(this.gameObject);

            //Rest of your Awake code
            //bulletPool = new List<GameObject>();  
            dupershot.SetActive(false);
        }
        else
        {
            Destroy(this);
        }       
    }
    
    public void SetDupershotActive(Transform shotPoint, int state)
    {
        var newBullet = dupershot;
        bulletController = newBullet.GetComponent<Bullet>();
        newBullet.transform.position = shotPoint.position;
        newBullet.SetActive(true);
        bulletController.DeterminateDirection(state);
    }

    public void SetBulletActive(Transform shotPoint, int state)
    {
        for (int i = 1; i < bulletPool.Count - 1; i++)
        {
            if (bulletPool[i].name == bulletPool[0].name)
            {
                bulletPool.RemoveAt(i);
                break;
            }
        }

        if (bulletPool.Count > 0)
        {
            var newBullet = bulletPool[0];
            bulletController = newBullet.GetComponent<Bullet>();
            newBullet.transform.position = shotPoint.position;
            newBullet.SetActive(true);
            bulletPool.Remove(newBullet);
            bulletController.DeterminateDirection(state);
        }        
    }

    public void ReturnToPool(GameObject bullet)
    {
        bulletPool.Add(bullet);

        bullet.SetActive(false);
    }
}
