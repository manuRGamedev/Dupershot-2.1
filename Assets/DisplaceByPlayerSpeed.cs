using UnityEngine;

public class DisplaceByPlayerSpeed : MonoBehaviour
{
    PlayerController player;

    // Update is called once per frame
    private void Start()
    {
        player = PlayerController.instance;
    }

    void Update()
    {
        transform.Translate((-1) * player.moveDir * player.movementSpeed * Time.deltaTime);
    }
}
