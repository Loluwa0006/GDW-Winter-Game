using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Run : StateMachineBehaviour
{
    [SerializeField] float accleration = 0.4f;
    [SerializeField] float max_speed = 2.5f;
    Rigidbody2D _rb;
    PlayerInput playerInput;
    PlayerController playerController;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.Play(layerIndex);
        _rb = animator.GetComponentInParent <Rigidbody2D>();
        playerInput = animator.GetComponentInParent<PlayerInput>();
        playerController = animator.GetComponentInParent<PlayerController>();

    }
    
    

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int move_dir = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);
        Vector2 newSpeed = _rb.linearVelocity;
        newSpeed.x = _rb.linearVelocity.x + accleration * move_dir;
        newSpeed.x = Mathf.Clamp(newSpeed.x, -max_speed, max_speed);

        _rb.linearVelocity = newSpeed * Time.deltaTime;

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
