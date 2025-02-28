using UnityEngine;

public class Player_Jump : AirState
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

       
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y + jumpVelocity);
        animator.SetBool("IsGrounded", false);
        animator.SetFloat("JumpBuffer", 0.0f);
        //reset buffer every time you jump so player doesn't accidently jump again
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 newSpeed = _rb.linearVelocity;
        int move_dir = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);
        if (newSpeed.magnitude < maxSpeed || Mathf.Sign(newSpeed.x) != move_dir)
        {
            newSpeed.x += acceleration * move_dir;
            newSpeed.x = Mathf.Clamp(newSpeed.x, -maxSpeed, maxSpeed);

        }

        newSpeed.y += getGravity() * Time.deltaTime;
        if (newSpeed.y > maxFallSpeed)
        {
            newSpeed.y = maxFallSpeed;
        }

        _rb.linearVelocity = newSpeed;


        animator.SetBool("MovingUpwards", (_rb.linearVelocity.y > 0));
        animator.SetBool("TouchingWall", TouchingWall());

       


        setFacing();

    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //  {
    // animator.SetFloat("JumpBuffer", -1.0f);
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

