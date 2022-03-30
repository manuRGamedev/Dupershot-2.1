using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("General Stats")]
    public PlayerState playerState;
    [SerializeField] bool autoChargeDupershot = false;

    [Header("Movement Stats")]
    public float movementSpeed;//Velocidad de movimiento
    public float startMovementSpeed = 4;//Velocidad de movimiento
    [HideInInspector] public int movementState; //Estado sobre el modo de movimiento
    [SerializeField] float speedIncreaseValue = 0.1f;
    [SerializeField] float maxMovementSpeed = 10f;
    
    bool isPlaying;

    [Header("Shooting Stats")]
    [SerializeField] float cooldown = 0.3f;
    float auxCooldown;
    [SerializeField]float shootInputRememberTime = 0.2f;
    float auxShootRememberTime;
    bool canShoot;

    [Header("Weapon Stats")]
    [SerializeField] Transform weaponPoint;

    [Header("Heat Control Stats")]    
    [SerializeField] float heatIncrease;
    [SerializeField] float heatReduction;
    [HideInInspector] public float heatStack;
    [SerializeField] float heatStackIncrease = 1f;
    [SerializeField] float heatStackDecreaseMultiplier = 0.85f;
    [SerializeField] float stackTime = 1.2f;
    float stackTimer;
    [HideInInspector] public float heatValue;
    [SerializeField] float overloadTime = 1.5f;

    [Header("Dupershot Stats")]
    [SerializeField] float dupershotIncrease;
    [SerializeField] float dupershotIncreaseConstant;
    [SerializeField] float maxDupershotIncreaseMultiplier = 25;
    [SerializeField] float heatCoolTime;
    [HideInInspector] public float dupershotValue;


    [Header("Animation Stats")]
    [SerializeField] float recoilDuration;
    [SerializeField] AnimationCurve recoilProgression;

    Animator anim;
    Rigidbody2D rb;
    [HideInInspector]public Vector2 moveDir;

    [Header("Elementos jugador")]
    [SerializeField] GameObject visualPlayer;
    [SerializeField] GameObject effectsObject;

    [Header("Eventos")]
    [SerializeField] UnityEvent OnShotPerformed;
    [SerializeField] UnityEvent OnShotFailed;
    [SerializeField] UnityEvent OnDupershotCharged;
    [SerializeField] UnityEvent OnDupershotReleased;
    [SerializeField] UnityEvent OnOverloadStarting;
    [SerializeField] UnityEvent OnOverloadEnding;
    [SerializeField] UnityEvent OnHitTaken;
    [SerializeField] UnityEvent OnResurrection;
    [SerializeField] UnityEvent OnReset;

    //AUX VARIABLES
    Coroutine recoilCorroutine;
    float lastStoredRotation;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        //Rest of your Awake code
        rb = GetComponent<Rigidbody2D>();
        //Sets game manager conections with the gameState event
        GameManager.OnGameStateChanged += SetPlaying;

        //Inicializar variables de Gameplay
        heatValue = 0f;
        stackTimer = 0f;
    }

    public void Start()
    { 
        movementSpeed = startMovementSpeed;
        movementState = 1;
        visualPlayer.transform.rotation = Quaternion.Euler(0,0,0);
        DeterminateMovement();
        auxShootRememberTime = 0;

        WasteDupershot();

        heatValue = 0;
        heatStack = 0;

        if (autoChargeDupershot)
        {
            dupershotValue = 100;
            ChargeDupershot();
        }

        OnReset.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        //Input management
        if (auxShootRememberTime > 0)
        {
            auxShootRememberTime -= Time.deltaTime;
        }

        //Cooldown Management
        if (auxCooldown > 0) auxCooldown -= Time.deltaTime;

        switch (GameManager.instance.state)
        {
            case GameState.Paused:
                break;
            case GameState.Resuming:
                break;
            default:
                //Gestiona los stacks de calor
                if (heatValue > 0) heatValue -= Mathf.Clamp(heatReduction - heatStack, 0, Mathf.Infinity) * Time.deltaTime;
                //heatStack = Mathf.Clamp(heatStack - heatStackIncrease * Time.deltaTime / 2, 0, Mathf.Infinity) ;

                if (stackTimer > 0)
                {
                    stackTimer -= Time.deltaTime;
                    if (stackTimer <= 0)
                    {
                        if (heatStack <= 2.5f)
                        {
                            heatStack = 0;
                        }
                        else
                        {
                            heatStack *= heatStackDecreaseMultiplier;
                            stackTimer = stackTime;
                        }
                    }
                }
                break;
        }

        //STATE MACHINE MANAGEMENT

        switch (playerState)
        {
            case PlayerState.Flying:
                
                //Manage To shoot
                if (auxShootRememberTime > 0 && auxCooldown <= 0) { Shoot(); }

                break;            

            case PlayerState.Stunned:
                if (auxShootRememberTime <= 0 && auxCooldown <= 0) { OnShotFailed.Invoke(); }
                break;

            case PlayerState.Dead:

                break;
        }        
    }
    
    public void WasteDupershot()
    {
        dupershotValue = 0;
    }

    public void SetNewState(PlayerState newState)
    {
        if (newState == playerState) { return; }

        switch (newState)
        {
            case PlayerState.Flying:

                break;

            case PlayerState.Inmune:

                break;

            case PlayerState.Stunned:

                break;

            case PlayerState.Dead:

                break;
        }

        playerState = newState;
    }

    public void AttemptShoot()
    {
        if (isPlaying)
        {
            auxShootRememberTime = shootInputRememberTime;
        }        
    }

    public void ResetCannon()
    {

    }

    public void Shoot()
    {
        //Gestiona Input
        auxShootRememberTime = 0f;

        //Genera un camShake    
        CamShakeController.instance.StartCoroutine(CamShakeController.instance.ShotShake(movementState));

        //Evita que se solapen las corrutinas
        if (recoilCorroutine != null) { StopCoroutine(recoilCorroutine); }

        //Determina el nuevo estado de movimiento
        movementState++;
        if (movementState > 4) movementState = 1;
        DeterminateMovement();
        //Dispara
        BulletPool.instance.SetBulletActive(weaponPoint, movementState);

        //Anima el retroceso
        recoilCorroutine = StartCoroutine(AnimateRecoil(movementState));
        
        //Establece un cooldown para evitar que se llame de nuevo a este método
        auxCooldown = cooldown;

        //Calienta el cañon
        WarmUpCannon();

        OnShotPerformed.Invoke();
    }

    public void AttemptDupershot()
    {
        if (dupershotValue >= 100)
        {
            MakeDupershot();
        }
    }

    public void MakeDupershot()
    {
        //Genera un camShake
        CamShakeController.instance.StartCoroutine(CamShakeController.instance.ShotShake(movementState));

        //Evita que se solapen las corrutinas
        if (recoilCorroutine != null) { StopCoroutine(recoilCorroutine); }

        //Determina el nuevo estado de movimiento
        movementState++;
        if (movementState > 4) movementState = 1;

        //Dispara
        BulletPool.instance.SetDupershotActive(weaponPoint, movementState);        

        //Establece un cooldown para evitar que se llame de nuevo a este método
        auxCooldown = cooldown;

        //Resetear calor
        stackTimer = 0;
        heatStack = 0;

        dupershotValue = 0;

        StartCoroutine(CoolDownCannon());

        OnDupershotReleased.Invoke();
    }

    public void ChargeDupershot()
    {
        dupershotValue += dupershotIncrease + dupershotIncreaseConstant * Mathf.Clamp(ScoreManager.instance.currentCombo, 1, maxDupershotIncreaseMultiplier);

        if (dupershotValue >= 100)
        {
            OnDupershotCharged.Invoke();
        }
    }

    void WarmUpCannon()
    {
        stackTimer = stackTime;
        heatStack += heatStackIncrease;
        heatValue += heatIncrease * heatStack;

        if (heatValue >= 100)
        {
            StartCoroutine(SetOverload());
            OnOverloadStarting.Invoke();
        }
    }

    void DeterminateMovement()
    {
        //Determina la nueva dirección de movimiento
        switch (movementState)
        {
            case 1:
                moveDir = Vector2.up;                
                break;
            
            case 2:
                moveDir = Vector2.right;
                break;

            case 3:
                moveDir = Vector2.down;
                break;

            case 4:
                moveDir = Vector2.left;
                break;
        }
    }

    public void IncreaseSpeed()
    {
        movementSpeed = Mathf.Clamp(movementSpeed + speedIncreaseValue, 4, maxMovementSpeed) ;
    }
    
    IEnumerator AnimateRecoil(int state)
    {
        Quaternion actualRotation = Quaternion.identity;

        //actualRotation = Quaternion.Euler(visualPlayer.transform.rotation.x, visualPlayer.transform.rotation.y, lastStoredRotation);

        float newAngle = 0f;
        float totalRotation = 0f;

        switch (state)
        {
            case 1:
                visualPlayer.transform.rotation = Quaternion.Euler(visualPlayer.transform.rotation.x, visualPlayer.transform.rotation.y, -270);
                effectsObject.transform.rotation = Quaternion.Euler(visualPlayer.transform.rotation.x, visualPlayer.transform.rotation.y, -270);
                newAngle = -360;
                break;
            
            case 2:
                visualPlayer.transform.rotation = Quaternion.Euler(visualPlayer.transform.rotation.x, visualPlayer.transform.rotation.y, 0);
                effectsObject.transform.rotation = Quaternion.Euler(visualPlayer.transform.rotation.x, visualPlayer.transform.rotation.y, 0);
                newAngle = -90;
                break;

            case 3:
                visualPlayer.transform.rotation = Quaternion.Euler(visualPlayer.transform.rotation.x, visualPlayer.transform.rotation.y, -90);
                effectsObject.transform.rotation = Quaternion.Euler(visualPlayer.transform.rotation.x, visualPlayer.transform.rotation.y, -90);
                newAngle = -180;
                break;

            case 4:
                visualPlayer.transform.rotation = Quaternion.Euler(visualPlayer.transform.rotation.x, visualPlayer.transform.rotation.y, -180);
                effectsObject.transform.rotation = Quaternion.Euler(visualPlayer.transform.rotation.x, visualPlayer.transform.rotation.y, -180);
                newAngle = -270;
                break;
        }

        float startRotation = visualPlayer.transform.eulerAngles.z;

        float time = 0;
        Quaternion newRotation = Quaternion.Euler(0, visualPlayer.transform.rotation.y, newAngle);
        

        while (time < recoilDuration)
        {
            //visualPlayer.transform.rotation = Quaternion.Lerp(actualRotation, newRotation, time / recoilDuration);

            visualPlayer.transform.rotation = Quaternion.Euler(visualPlayer.transform.rotation.x, visualPlayer.transform.rotation.y, startRotation + (-90) * recoilProgression.Evaluate(time / recoilDuration));

            //visualPlayer.transform.Rotate(Vector3.forward * totalRotation/recoilDuration * Time.deltaTime);

            //visualPlayer.transform.Rotate(Vector3.forward * ((-90 * (recoilProgression.Evaluate(time/recoilDuration)))/recoilDuration) * Time.deltaTime);
            
            time +=  Time.deltaTime;

            yield return null;
        }

        visualPlayer.transform.rotation = newRotation;
        effectsObject.transform.rotation = newRotation;
        lastStoredRotation = newAngle;
        if (lastStoredRotation == -360) { lastStoredRotation = 0f;}

               
        
    }

    public void RestoreOverload()
    {
        if (playerState == PlayerState.Stunned)
        {
            //Se ajustan los valores del calor y los stack
            heatValue = 0;
            heatStack = 0;
            //Se vuelve al estado inicial
            SetNewState(PlayerState.Flying);
            OnOverloadEnding.Invoke();
        }
    }
    
    IEnumerator SetOverload()
    {
        SetNewState(PlayerState.Stunned);
        //Se realiza una cuenta atras para reiniciar la sobrecarga
        float overloadCountdown = overloadTime;
        do
        {
            switch (GameManager.instance.state)
            {
                case GameState.Paused:
                    break;
                case GameState.Resuming:
                    break;
                default:
                    overloadCountdown -= Time.deltaTime;
                    break;
            }            
            yield return null;
        }
        while (overloadCountdown > 0f);

        if (playerState == PlayerState.Stunned)
        {
            //Se ajustan los valores del calor y los stack
            heatValue = 0;
            heatStack = 0;
            //Se vuelve al estado inicial
            SetNewState(PlayerState.Flying);
            OnOverloadEnding.Invoke();
        }
    }

    IEnumerator CoolDownCannon()
    {
        float heatToCool = heatValue;

        float time = 0f;

        while (time <= heatCoolTime)
        {
            heatValue -= (heatToCool / heatCoolTime) * Time.deltaTime;
            time += Time.deltaTime;
            yield return null;
        }

        heatValue = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Enemy":
                OnHitTaken.Invoke();
                break;

            default:
                break;
        }
    }

    public virtual void SetPlaying(GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing:
                isPlaying = true;
                break;
            case GameState.Frozen:
                isPlaying = false;
                break;
            case GameState.Paused:
                isPlaying = false;
                break;
            case GameState.Resuming:
                isPlaying = false;
                break;
            case GameState.Resurrecting:
                isPlaying = false;
                break;
            case GameState.GameOver:
                isPlaying = true;
                break;
        }
    }

    public void DelayShoot(float delayTime)
    {
        IEnumerator DelayCorroutine(float time)
        {
            yield return new WaitForSeconds(time);
           
            DeterminateMovement();
            //Anima el retroceso
            recoilCorroutine = StartCoroutine(AnimateRecoil(movementState));
        }

        StartCoroutine(DelayCorroutine(delayTime));
    }
}

public enum PlayerState { Flying, Stunned, Dead, Inmune };
