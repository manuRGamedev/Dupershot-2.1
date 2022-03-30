using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    //Componentes
    protected Rigidbody2D rb;
    //Variables de moimiento
    protected Vector2 worldMoveDir;
    protected Vector2 moveDir = Vector2.zero;
    //Gestión de Pool
    protected List<GameObject> myPool;
    protected bool isPlaying;
    

    [Tooltip("Indice de la pool de Ashes a la que se llamará cuando el personaje sea destruido")]
    [HideInInspector]public int ID;
    [HideInInspector] public int sizeInPool = 1;

    [Header("Movement")]
    [SerializeField] protected float movementSpeed;
    protected float currentMovementSpeed;
    protected float startMovementSpeed;
    [SerializeField] protected SpriteRenderer spriteContainer;

    protected PlayerController player;


    // Start is called before the first frame update
    protected virtual void Awake()
    {
        player = PlayerController.instance;
        rb = GetComponent<Rigidbody2D>();
    }
    public void Initialize()
    {
        currentMovementSpeed = movementSpeed;
    }

    protected virtual void OnEnable()
    {
        if(player == null) { player = PlayerController.instance; }

        GetMoveDir(player.gameObject.transform.position);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isPlaying)
        {
            worldMoveDir = (-1) * player.moveDir * player.movementSpeed;
            transform.Translate((moveDir * currentMovementSpeed + worldMoveDir) * Time.deltaTime);
        }
    }

    public virtual void increaseSpeed()
    {
        currentMovementSpeed += startMovementSpeed * 0.1f;
    }

    public virtual void GetMoveDir(Vector3 targetPos)
    {
        float x = Random.Range(-1f, 1f);
        float y = Random.Range(-1f, 1f);
        Vector3 errorMargin = new Vector3(x, y, 0);
        moveDir = (targetPos + errorMargin - transform.position).normalized;
    }

    public virtual void AsignSprite(Sprite sprite, Vector2 scale)
    {
        spriteContainer.sprite = sprite;
        spriteContainer.gameObject.transform.localScale = new Vector3(scale.x, scale.y, 1);
        float randRot = Random.Range(0, 360f);

        spriteContainer.transform.eulerAngles = new Vector3(0, 0, randRot);
    }

    public void SetPool(List<GameObject> objectPool)
    {
        myPool = objectPool;
    }

    protected virtual void SetDefeated()
    {
        EnemyPool.instance.SetAshesActive(transform.position, ID);
        CamShakeController.instance.StartCoroutine(CamShakeController.instance.CamShake());
        ScoreManager.instance.ReportHit(transform.position);

        SetBackToPool();
    }

    public void SetBackToPool()
    {
        if (myPool != null) { EnemyPool.instance.ReturnToPool(gameObject, myPool, sizeInPool); }
    }

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        var x = transform.position.x;
        var y = transform.position.y;

        switch (collision.tag)
        {
            case "Bounds":
                EnemyPool.instance.ReturnToPool(gameObject, myPool, sizeInPool);
                break;

            case "PlayerBullet":
                SetDefeated();
                break;

            case "DuperBullet":
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

    protected void OnTriggerExit2D(Collider2D collision)
    {
        var x = transform.position.x;
        var y = transform.position.y;

        switch (collision.tag)
        {
            case "Bounds":
                //EnemyPool.instance.ReturnToPool(gameObject, myPool);
                break;

            default:
                break;
        }
    }

    public void StopMoving()
    {
        isPlaying = false;
    }

    public void StartMoving()
    {
        isPlaying = true;
    }
}
