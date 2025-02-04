using UnityEngine;
using UnityEngine.InputSystem;

public class Base_State : StateMachineBehaviour
{
    protected PlayerInput playerInput;
    protected PlayerController playerController;

    [SerializeField] protected bool useFixedUpdate = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.Play(layerIndex);
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

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int move_dir = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);
        animator.SetInteger("HorizAxis", move_dir);

        bool crouch_held = playerInput.actions["Crouch"].IsPressed();
        animator.SetBool("CrouchHeld", crouch_held);

        animator.SetBool("IsGrounded", playerController.isGrounded());
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
