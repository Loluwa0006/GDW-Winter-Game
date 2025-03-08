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

   protected LayerMask groundMask ;



   protected bool stateInitalized = false;

    protected ShadowStrideControls _ssControls;

    BoxCollider2D playerControllerHitbox;
    Vector2 groundColliderSize;



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

            playerControllerHitbox = playerController.GetHurtbox();
            InitInputActions(animator);
            //Shrink ground collider size to make sure player is standing on top of something
            //Without this the player would stick to walls by having the furthest parts of the model touch said wall

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
        animator.SetBool("IsGrounded", TouchingGround());
        setFacing();


    }

    public bool TouchingGround()
    {
        RaycastHit2D hit = Physics2D.BoxCast(playerController.transform.position, playerController.groundColliderSize, 0, new Vector2(0, -1), GROUND_CHECKER_LENGTH, groundMask);
            return hit;
    }

 




 




    public virtual bool IsGrounded()
    {
        return TouchingGround();
    }
        playerInput.actions["Attack"].started += ctx => animator.SetTrigger("AttackPressed");

        //_ssControls.FindAction("Move").performed += ctx => animator.SetInteger("HorizAxis", Mathf.RoundToInt(ctx.ReadValue<Vector2>().x));
    protected void setFacing()
    {
        int move_dir = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);
        if (move_dir == 0) { move_dir = Mathf.RoundToInt(playerController.transform.localScale.x); }
        playerController.transform.localScale = new Vector2(move_dir, 1);
    }

   protected virtual void InitInputActions(Animator animator)
    {
        //_ssControls.FindAction("Move").performed += ctx => animator.SetInteger("HorizAxis", Mathf.RoundToInt(ctx.ReadValue<Vector2>().x));
    }

   
    //}
}
