using Unity.VisualScripting;
using UnityEngine;

public class MoveState : Base_State
{

    const float COYOTE_DURATION = 20f;

    Rigidbody2D _rb;

    [SerializeField] float acceleration = 0.5f;
    [SerializeField] float max_speed = 2.5f;
     Timer jumpBuffer;
    Timer attackBuffer;
    Timer coyoteTimer;

    bool groundedLastFrame = false;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
         
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (_rb == null)
        {
            _rb = animator.GetComponentInParent<Rigidbody2D>();
        }
        if (jumpBuffer == null)
        {
            jumpBuffer = playerController.GetTimer("JumpBuffer");
        }
        if (coyoteTimer == null)
        {
            coyoteTimer = playerController.GetTimer("Coyote");
        }
        if (attackBuffer == null)
        {
            attackBuffer = playerController.GetTimer("AttackBuffer");
        }

    }



    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int move_dir = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);
        Vector2 newSpeed = _rb.linearVelocity;
        newSpeed.x = _rb.linearVelocity.x + acceleration * move_dir;
        newSpeed.x = Mathf.Clamp(newSpeed.x, -max_speed, max_speed);

        _rb.linearVelocity = newSpeed;

        animator.SetInteger("HorizAxis", move_dir);


        bool crouchHeld = playerInput.actions["Crouch"].IsPressed(); 
        bool attackPressed = playerInput.actions["Attack"].IsPressed();

        if (attackPressed)
        {
            attackBuffer.StartTimer();
        }

        animator.SetFloat("AttackBuffer", attackBuffer.timeRemaining());
        animator.SetBool("CrouchHeld", crouchHeld);

        animator.SetBool("IsGrounded", isGrounded());

        if (playerInput.actions["Jump"].IsPressed())
        {
            jumpBuffer.StartTimer();
        }
        animator.SetFloat("JumpBuffer", jumpBuffer.timeRemaining());
        if (groundedLastFrame && !touchingGround())
        {
            coyoteTimer.StartTimer();
        }

        if (move_dir == 0) { move_dir = Mathf.RoundToInt(playerController.transform.localScale.x); }
        Debug.Log("Setting scale to " + move_dir.ToString());
        playerController.gameObject.transform.localScale = new Vector2(move_dir, 1);

        Debug.Log(playerController.gameObject.name);

        setFacing();
    }


    public override bool isGrounded()
    {
        if (!touchingGround() && coyoteTimer.isStopped())
        {
            coyoteTimer.StartTimer(COYOTE_DURATION, false);
        }
        if (touchingGround() && !coyoteTimer.isStopped())
        {
            return true;
        }
        return false;

    }

}
