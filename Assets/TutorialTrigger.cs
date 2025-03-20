using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{

    [SerializeField] TutorialManager tutorialManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        tutorialManager.conditionTracker += 1;
        gameObject.SetActive(false);
    }
}
