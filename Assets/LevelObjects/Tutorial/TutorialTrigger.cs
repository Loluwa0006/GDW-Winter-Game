using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{

    [SerializeField] TutorialManager tutorialManager;

    private void Awake()
    {
        if (tutorialManager ==  null)
        {
            tutorialManager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<TutorialManager>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        tutorialManager.conditionTracker += 1;
        gameObject.SetActive(false);
    }
}
