using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Enemy
{
    [Header("Visual Stats")]
    [SerializeField] GameObject strongVersion;
    [SerializeField] float strongSpeed;
    [SerializeField] GameObject weakVersion;
    [SerializeField] float weakSpeed;
    [SerializeField] ParticleSystem crystalParticles;
    [SerializeField] CallAudioManagerProperty audioCall;

    bool acorazado;
    
    protected override void OnEnable()
    {
        acorazado = true;
        strongVersion.SetActive(true);
        weakVersion.SetActive(false);
        movementSpeed = strongSpeed;

        if (player == null) { player = PlayerController.instance; }

        GetMoveDir(player.gameObject.transform.position);
    }

    // Update is called once per frame
    protected override void Update()
    {     

        if (isPlaying)
        {
            //player = PlayerController.instance;
            worldMoveDir = (-1) * player.moveDir * player.movementSpeed;
            transform.Translate((moveDir * currentMovementSpeed + worldMoveDir) * Time.deltaTime);
        }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        var x = transform.position.x;
        var y = transform.position.y;

        switch (collision.tag)
        {
            case "Bounds":
                EnemyPool.instance.ReturnToPool(gameObject, myPool,sizeInPool);
                break;

            case "PlayerBullet":
                if (acorazado)
                {
                    crystalParticles.Play();
                    acorazado = false;
                    strongVersion.SetActive(false);
                    weakVersion.SetActive(true);
                    movementSpeed = weakSpeed;
                    CamShakeController.instance.StartCoroutine(CamShakeController.instance.CamShake());
                    audioCall.PlayDelayedList();
                    ScoreManager.instance.ReportHit(transform.position);
                }
                else
                {
                    SetDefeated();
                }                
                break;

            case "DuperBullet":
                AudioManager.instance.Play("Impact_Crystals");
                crystalParticles.Play();
                acorazado = false;
                strongVersion.SetActive(false);
                weakVersion.SetActive(true);
                movementSpeed = weakSpeed;
                CamShakeController.instance.StartCoroutine(CamShakeController.instance.CamShake());
                ScoreManager.instance.ReportSuperHit(transform.position);

                EnemyPool.instance.SetAshesActive(transform.position, ID);
                CamShakeController.instance.StartCoroutine(CamShakeController.instance.CamShake());
                ScoreManager.instance.ReportSuperHit(transform.position);                

                SetBackToPool();
                break;

            case "Breaker":
                EnemyPool.instance.SetAshesActive(transform.position, ID);
                CamShakeController.instance.StartCoroutine(CamShakeController.instance.CamShake());

                SetBackToPool();
                break;

            default:
                break;
        }
    }
}
