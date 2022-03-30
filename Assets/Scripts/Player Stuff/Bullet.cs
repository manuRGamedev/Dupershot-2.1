using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed;
    [SerializeField] Transform projectileTransform;
    [SerializeField] int maxImpacts = 1;
    [SerializeField] float bulletSpawnDelay = 0f;
    float delayTime = 0f;
    int impacts;
    bool isPlaying = false;
    bool hasLaunched = false;
    [SerializeField] bool ownedByPool = true;
    [HideInInspector] public Vector2 moveDir;
    Vector3 bulletStartPosition;

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        bulletStartPosition = projectileTransform.localPosition;
        rb = GetComponent<Rigidbody2D>();        
    }

    private void OnEnable()
    {
        hasLaunched = false;
        projectileTransform.localPosition = bulletStartPosition;
        impacts = maxImpacts;
        delayTime = bulletSpawnDelay;
        projectileTransform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaying)
        {
            if (!hasLaunched)
            {
                delayTime -= Time.deltaTime;

                if (delayTime <= 0)
                {
                    hasLaunched = true;
                    projectileTransform.gameObject.SetActive(true);
                    projectileTransform.localPosition = bulletStartPosition;
                }
            }
            else
            {
                projectileTransform.position += (Vector3)(moveDir * bulletSpeed - PlayerController.instance.moveDir * PlayerController.instance.movementSpeed) * Time.deltaTime;

                if (Vector3.Distance(PlayerController.instance.transform.position, projectileTransform.position) > 20f)
                {
                    SetInactive();
                }
            }
        }            
    }

    void MakeImpact()
    {
        impacts--;

        if (impacts == 0)
        {
            SetInactive();
        }
    }

    public void DeterminateDirection(int shotState)
    {
        switch (shotState)
        {
            case 1:
                moveDir = Vector2.down;
                transform.eulerAngles = new Vector3(0, 0, 90);
                break;

            case 2:
                moveDir = Vector2.left;
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;

            case 3:
                moveDir = Vector2.up;
                transform.eulerAngles = new Vector3(0, 0, -90);
                break;

            case 4:
                moveDir = Vector2.right;
                transform.eulerAngles = new Vector3(0, 0, 180);
                break;
        }
    }

    void SetInactive()
    {
        if (ownedByPool)
        {
            BulletPool.instance.ReturnToPool(this.gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Enemy":
                MakeImpact();
                break;

            default:
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Bounds":
                SetInactive();
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
