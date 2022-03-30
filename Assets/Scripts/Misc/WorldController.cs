using UnityEngine;

public class WorldController : MonoBehaviour
{
    [SerializeField] Material backgroundMaterial;
    [SerializeField] [Range(0.001f, 1f)] float backgroundSpeed = 0.005f;

    bool isMoving;

    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.instance;
        ResetWorldOffset();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) { return; }

        var offset = backgroundMaterial.mainTextureOffset;
        backgroundMaterial.mainTextureOffset += player.moveDir * player.movementSpeed * backgroundSpeed * Time.deltaTime;
        //backgroundMaterial.mainTextureOffset = new Vector2(Mathf.Clamp(offset.x, 0f, 1f), Mathf.Clamp(offset.y, 0f, 1f));
    }

    public void SetMovement(bool value)
    {
        isMoving = value;
    }

    public void ResetWorldOffset()
    {
        backgroundMaterial.mainTextureOffset = Vector2.zero;
    }
}
