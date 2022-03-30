using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityIndicator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Sprite indicatorSprite;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color indicatorColor = Color.red;
    [SerializeField] Transform indicatorTransform;

    [Header("Screen Positioning")]
    [SerializeField] bool canRotate = true;
    [SerializeField][InspectorName("Distance Margin")] float distance = 5.5f;

    [Header("Other")]
    [SerializeField] bool isWarning = false;

    bool warningEnded;

    Transform player;
    private bool indicatorIsActive;

    // Start is called before the first frame update
    void Start()
    {
        //Inicializa el transform del jugador
        player = PlayerController.instance.transform;

        //Inicializa el renderer
        spriteRenderer.color = indicatorColor;
    }

    private void OnEnable()
    {
        warningEnded = false;
        SetIndicatorActive(true);        
    }

    // Update is called once per frame
    void Update()
    {
        if (indicatorIsActive)
        {
            //Check the state of the enemy position
            float x = 0;
            float y = 0;

            if (Mathf.Abs(player.position.x - transform.position.x) > Mathf.Abs(player.position.y - transform.position.y))
            {
                if (transform.position.x > 0)
                {
                    x = distance + player.position.x;
                    y = transform.position.y;
                    
                    if (canRotate) indicatorTransform.rotation = Quaternion.Euler(0f,0f,-90f);
                }
                else
                {
                    x = -distance + player.position.x;
                    y = transform.position.y;

                    if (canRotate) indicatorTransform.rotation = Quaternion.Euler(0f, 0f, 90f);
                }
            }
            else
            {
                if (transform.position.y > 0)
                {
                    x = transform.position.x;
                    y = distance + player.position.y;

                    if (canRotate) indicatorTransform.rotation = Quaternion.Euler(0f, 0f, 0);
                }
                else
                {
                    x = transform.position.x;
                    y = -distance + player.position.y;

                    if (canRotate) indicatorTransform.rotation = Quaternion.Euler(0f, 0f, 180);
                }
            }

            indicatorTransform.position = new Vector3(Mathf.Clamp(x, -distance + player.position.x, distance + player.position.x), Mathf.Clamp(y, -distance + player.position.y, distance + player.position.y), 0f);
        }
    }

    public void SetIndicatorActive(bool value)
    {  
        if (value)
        {
            if (!warningEnded) { warningEnded = true; }
            else if (isWarning) { return; }
        }

        indicatorIsActive = value;

        if (value)
        {
            spriteRenderer.sprite = indicatorSprite;
        }
        else
        {
            spriteRenderer.sprite = null;
        }        
    }


}
