using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Chaser : Enemy
{
   
    [SerializeField] Transform rotator;
    Transform target;
    [SerializeField] Animator chaserAnim;

    [Header("Movement Stats")]
    [SerializeField] float minTurnSpeed = 15f;
    [SerializeField] float maxTurnSpeed = 45f;
    [SerializeField] float minDistance = 1f;
    [SerializeField] float maxDistance = 8;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerController.instance.transform;
        player = PlayerController.instance;
        moveDir = Vector2.up;
    }
    // Update is called once per frame
    protected override void Update()
    {
        //Define su dirección de movimiento
        SetMoveDirection();

        //Anima el enemigo en función de la animación
        chaserAnim.SetFloat("HorizontalSpeed", moveDir.x);
        chaserAnim.SetFloat("VerticalSpeed", moveDir.y);



        if (isPlaying)
        {
            //player = PlayerController.instance;
            worldMoveDir = (-1) * player.moveDir * player.movementSpeed;
            transform.Translate((moveDir * currentMovementSpeed + worldMoveDir) * Time.deltaTime);
        }
    }

    protected override void OnEnable()
    {
        if (player == null) { player = PlayerController.instance; }
        Vector3 aim = player.transform.position - transform.position;
        rotator.eulerAngles = new Vector3(0,0,360 * Mathf.Atan2(aim.y, aim.x)/(2 * Mathf.PI));
        Debug.Log("Angle = " + (360 * Mathf.Atan2(aim.y, aim.x) / (2 * Mathf.PI)).ToString());
    }

    protected void SetMoveDirection()
    {
        Vector2 targetDir = target.position - transform.position;
        float distance = Vector2.Distance((Vector2)transform.position, (Vector2)target.position);

        float rotationAmount = Vector3.Cross(targetDir, rotator.up).z;
        float rotationSpeed = minTurnSpeed + (Mathf.Clamp(Mathf.Clamp(distance - minDistance, 0, maxDistance) / (maxDistance - minDistance), 0,1) * (maxTurnSpeed - minTurnSpeed));

        rotator.Rotate(-rotationAmount * rotationSpeed * rotator.forward * Time.deltaTime);

        moveDir = rotator.up.normalized;        
    }
}
