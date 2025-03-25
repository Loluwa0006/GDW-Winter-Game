using UnityEngine;

public class TutorialBoundary : MonoBehaviour
{

    [SerializeField] TutorialManager tutorialManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController controller = collision.GetComponent<PlayerController>();
        if (controller != null)
        {
            tutorialManager.ResetPlayerPosition();
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
