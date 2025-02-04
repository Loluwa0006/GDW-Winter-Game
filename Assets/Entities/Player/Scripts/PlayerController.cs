using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HealthComponent))]
[System.Serializable]
public class PlayerController : MonoBehaviour
{
    HealthComponent healthComponent;
    Animator animator;
    BoxCollider2D hurtbox;
    [SerializeField] Text state_traacker;
    [SerializeField] float ground_checker_length;
    [SerializeField] LayerMask groundMask;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        healthComponent = GetComponent<HealthComponent>();
        animator = GetComponent<Animator>();
        hurtbox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
     //   Debug.Log("Current state is now " + animator.GetCurrentAnimatorStateInfo(0).ToString());
    }

    public void UpdateStateTracker(string newState)
    {
        state_traacker.text = "State: " + newState;
    }

    public bool hasParameter(string parameterName)
    {
        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.parameters[i].name == parameterName)
            {
                return true;
            }
        }
        return false;
    }

    public bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, hurtbox.size, 0, new Vector2(0, -1), ground_checker_length, groundMask);
        return hit;
    }
}
