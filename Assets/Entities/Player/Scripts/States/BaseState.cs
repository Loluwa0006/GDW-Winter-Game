using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Base_State : StateMachineBehaviour
{
    protected const float GROUND_CHECKER_LENGTH = 0.5f;

    const float GROUND_CHECKER_RATIO = 0.8f;

    protected PlayerInput playerInput;
    protected PlayerController playerController;

    [SerializeField] protected bool useFixedUpdate = false;

   protected bool stateInitalized = false;

    BoxCollider2D playerControllerHitbox;
    Vector2 groundColliderSize;



    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state




    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInitalized)
        {
            playerInput = animator.GetComponentInParent<PlayerInput>();
            playerController = animator.GetComponent<PlayerController>();

            if (useFixedUpdate)
            {
                animator.updateMode = AnimatorUpdateMode.Fixed;
            }
            else
            {
                animator.updateMode = AnimatorUpdateMode.Normal;
            }
            stateInitalized = true;

            playerControllerHitbox = playerController.GetHurtbox();
            InitInputActions(animator);
            
        }

        animator.Play(layerIndex);




    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int move_dir = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);

        animator.SetBool("IsGrounded", TouchingGround());
        SetFacing();
    }

    public bool TouchingGround()
    {
        RaycastHit2D hit = Physics2D.BoxCast(playerController.transform.position, playerController.groundColliderSize, 0, new Vector2(0, -1), GROUND_CHECKER_LENGTH, playerController.groundMask);
        if (hit)
        {
            if (hit.transform == playerController.transform)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    public virtual bool IsGrounded()
    {
        return TouchingGround();
    }

        //_ssControls.FindAction("Move").performed += ctx => animator.SetInteger("HorizAxis", Mathf.RoundToInt(ctx.ReadValue<Vector2>().x));
    protected void SetFacing()
    {
        int move_dir = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);
        if (move_dir == 0) { move_dir = Mathf.RoundToInt(playerController.transform.localScale.x); }
        playerController.transform.localScale = new Vector2(move_dir, 1);
    }

   protected virtual void InitInputActions(Animator animator)
    {
        playerInput.actions["Attack"].performed += ctx => animator.SetTrigger("AttackPressed");
        playerInput.actions["Move"].performed += ctx => SetMovementAxis(animator, ctx.ReadValue<Vector2>());

        //_ssControls.FindAction("Move").performed += ctx => animator.SetInteger("HorizAxis", Mathf.RoundToInt(ctx.ReadValue<Vector2>().x));
    }


    void SetMovementAxis(Animator animator, Vector2 axis)
    {
        Vector2Int axisAsInt = new(Mathf.RoundToInt(axis.x), Mathf.RoundToInt(axis.y));

        animator.SetInteger("HorizAxis", axisAsInt.x);
        animator.SetInteger("VertAxis", axisAsInt.y);
        
    }

    //}
}
