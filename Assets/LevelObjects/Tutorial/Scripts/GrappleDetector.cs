using UnityEngine;

public class GrappleDetector : MonoBehaviour
{

    [SerializeField] TutorialManager manager;
    [SerializeField] bool detectTether = false;
    [SerializeField] bool detectHook = true;

    bool foundTarget;

    private void Awake()
    {
        if (manager == null)
        {
            manager = GameObject.FindGameObjectWithTag("TutorialManager").GetComponent<TutorialManager>();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, GetComponent<BoxCollider2D>().size, 0.0f);

        foreach (var collider in colliders)
        {
            if (collider.tag == "GrapplingHook" && detectHook)
            {
                Debug.Log("detected grapple");
                foundTarget = true;
            }
            if (collider.tag == "TetherPoint" && detectTether)
            {
                Debug.Log("detected tether");
                foundTarget = true;
            }
            if (foundTarget)
            {
                manager.conditionTracker += 1;
                gameObject.SetActive(false);
            }
        }
    }
}
