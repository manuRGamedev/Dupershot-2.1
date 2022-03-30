using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comet : Enemy
{
    [SerializeField] float travelSpeed;
    [SerializeField] float delayTime;
    [SerializeField]float holdDistance = 8.5f;

    private CircleCollider2D myCollider;

    float delayTimer;

    // Start is called before the first frame update
    void Start()
    {
        
        player = PlayerController.instance;
    }

    // Start is called before the first frame update
    protected override void OnEnable()
    {
        if (player == null) { player = PlayerController.instance; }

        myCollider = GetComponent<CircleCollider2D>();
        myCollider.enabled = false;
        delayTimer = 0;
        SetRotation();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (isPlaying)
        {
            worldMoveDir = (-1) * player.moveDir * player.movementSpeed;

            if (delayTimer < delayTime)
            {
                MoveOnHolding();

                delayTimer += Time.deltaTime;
                if (delayTimer >= delayTime) myCollider.enabled = true;

            }
            else
            {
                transform.Translate((moveDir * travelSpeed + worldMoveDir) * Time.deltaTime);
            }            
        }
    }

    void SetRotation()
    {
        var t = transform.position;
        var p = PlayerController.instance.transform.position;

        Vector3 dir = p - t;

         if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (p.x < 0) { moveDir = Vector2.right; }
            else {  moveDir = Vector2.left; }
        }
        else
        {
            if (t.y < 0) { moveDir = Vector2.up; }
            else { moveDir = Vector2.down; }
        }
    }

    void MoveOnHolding()
    {
        transform.Translate((moveDir * travelSpeed + worldMoveDir) * Time.deltaTime);

        var t = transform.position;
        var p = PlayerController.instance.transform.position;

        Vector3 dir = p - t;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (p.x < 0) { t = new Vector3(-holdDistance, t.y, 0); }
            else { t = new Vector3(holdDistance, t.y, 0); }
        }
        else
        {
            if (t.y < 0) { t = new Vector3(t.x, -holdDistance, 0); }
            else { t = new Vector3(t.x, holdDistance, 0); }
        }
    }

}
