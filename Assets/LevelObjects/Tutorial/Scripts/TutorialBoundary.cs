using UnityEngine;

public class TutorialBoundary : MonoBehaviour
{

    [SerializeField] TutorialManager tutorialManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            tutorialManager.ResetPlayerPosition();
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
}
