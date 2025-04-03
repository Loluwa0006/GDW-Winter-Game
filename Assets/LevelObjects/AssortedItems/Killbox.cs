using UnityEngine;

public class Killbox : MonoBehaviour
{
   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController playerController))
        {
            if (!playerController.deadAF)
            {
                playerController.OnPlayerDeath();
            }
        }
        else
        {
            if (collision.tag != "ItemDropper")
            {
               Destroy(collision.gameObject);
            }
        }
    }
}
