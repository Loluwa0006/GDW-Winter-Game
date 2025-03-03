using UnityEngine;

public class Killbox : MonoBehaviour
{
   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.onPlayerDeath();

        }
    }
}
