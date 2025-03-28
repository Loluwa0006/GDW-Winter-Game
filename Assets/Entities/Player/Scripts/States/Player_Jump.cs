using UnityEngine;

public class Player_Jump : AirState
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

       
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpVelocity);
        animator.SetBool("IsGrounded", false);
        animator.SetBool("MovingUpwards", true);
        animator.SetFloat("JumpBuffer", 0.0f);
        jumpBuffer.StopTimer();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 newSpeed = _rb.linearVelocity;
        int move_dir = animator.GetInteger("HorizAxis");
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


        if (_rb.linearVelocity.y < 0)
        {
            animator.SetBool("MovingUpwards", false);
        }
        animator.SetBool("TouchingWall", TouchingWall());

       


        SetFacing();

    }
    
}

