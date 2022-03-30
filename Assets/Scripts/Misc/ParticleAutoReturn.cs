using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoReturn : MonoBehaviour
{
    [SerializeField] float lifetime = 2f;
    float auxLifetime;
    Enemy me;

    // Start is called before the first frame update
    void Awake()
    {
        me = GetComponent<Enemy>();
        auxLifetime = lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if (auxLifetime >= 0)
        {
            auxLifetime -= Time.deltaTime;

            if (auxLifetime <= 0)
            {
                EnemyPool.instance.ReturnParticlesToPool(gameObject, EnemyPool.instance.particlePools[me.ID]);
            }
        }        
    }

    private void OnEnable()
    {
        auxLifetime = lifetime;
    }
}
