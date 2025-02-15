using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Base_State : StateMachineBehaviour
{
    const float GROUND_CHECKER_LENGTH = 0.5f;

    protected PlayerInput playerInput;
    protected PlayerController playerController;

    [SerializeField] protected bool useFixedUpdate = false;

   protected LayerMask groundMask ;



   protected bool stateInitalized = false;

    protected ShadowStrideControls _ssControls;

   
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInitalized)
        {
            groundMask = LayerMask.GetMask("Ground", "Wall");
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
            _ssControls = playerController._ssControls;


        }
        animator.Play(layerIndex);




    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int move_dir = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);
        animator.SetInteger("HorizAxis", move_dir);

        bool crouch_held = playerInput.actions["Crouch"].IsPressed();
        animator.SetBool("CrouchHeld", crouch_held);

        animator.SetBool("IsGrounded", isGrounded());

        setFacing();
    }


    public bool touchingGround()
    {
        BoxCollider2D playerControllerHitbox = playerController.GetHurtbox();
        Vector2 groundColliderSize = Vector2.Scale(playerControllerHitbox.size, new Vector2(0.8f, 0.8f));
            RaycastHit2D hit = Physics2D.BoxCast(playerController.transform.position, groundColliderSize, 0, new Vector2(0, -1), GROUND_CHECKER_LENGTH, groundMask);
            return hit;
        }
    
    public virtual bool isGrounded()
    {
  

        return touchingGround();
    }

    protected void setFacing()
    {
        int move_dir = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);
        if (move_dir == 0) { move_dir = Mathf.RoundToInt(playerController.transform.localScale.x); }
        playerController.transform.localScale = new Vector2(move_dir, 1);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
   
    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
