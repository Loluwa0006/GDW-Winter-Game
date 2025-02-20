using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HealthComponent))]
[System.Serializable]
public class PlayerController : MonoBehaviour
{
    Animator animator;
    BoxCollider2D hurtbox;
    [SerializeField] Text state_traacker;
    [SerializeField] float ground_checker_length;
    [SerializeField] LayerMask groundMask;

    [SerializeField] Text mouseTracker;
    [SerializeField] Text velocityTracker;

    Rigidbody2D _rb;
   [SerializeField] public  ShadowStrideControls _ssControls;

    public Transform _respawnPoint;
    public enum grappleElements
    {
        AIR,
        EARTH,
        FIRE,
        GRAVITY
    }
    public grappleElements grappleElement = grappleElements.AIR;


    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Awake()
    {
        animator = GetComponent<Animator>();
        hurtbox = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
        _ssControls = new ShadowStrideControls();
    }

    private void OnEnable()
    {
        if (_ssControls == null)
        {
            _ssControls = new ShadowStrideControls();
        }
        _ssControls.Enable();

        Debug.Log("enabled controls");
    }
    private void OnDisable()
    {
        _ssControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        mouseTracker.text = "Mouse Position: " + Camera.main.ScreenToWorldPoint(Input.mousePosition).ToString();
        velocityTracker.text = "Velocity: " + _rb.linearVelocity.ToString(); 
        //   Debug.Log("Current state is now " + animator.GetCurrentAnimatorStateInfo(0).ToString());
    }

    public void UpdateStateTracker(string newState)
    {
        state_traacker.text = "State: " + newState;
    }

    public int HasParameter(string parameterName)
    {
        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.parameters[i].name == parameterName)
            {
                return i;
            }
        }
        return -1;
    }

    public BoxCollider2D GetHurtbox()
    {
        return hurtbox;
    }

    public Timer GetTimer(string timerName)
    {
        foreach (Component component in GetComponentsInChildren<Timer>() )
        {
            Timer timer = (Timer) component;
            if (timer.GetID() == timerName)
            {
                return timer;
            }
        }
        Debug.Log("Couldn't find timer of name " + timerName);
        return null;
    }

    public void Respawn()
    {
        transform.position = _respawnPoint.position;

        //below is deepseek code
        // Loop through all parameters
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(param.name, 0f); // Reset float to 0
                    break;

                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(param.name, 0); // Reset int to 0
                    break;

                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(param.name, false); // Reset bool to false
                    break;

                case AnimatorControllerParameterType.Trigger:
                    animator.ResetTrigger(param.name); // Reset trigger
                    break;
            }

        }
        //deepseek code over

        animator.SetBool("IsGrounded", false);

    }
}
