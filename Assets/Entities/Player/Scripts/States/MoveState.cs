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

    float desiredSpeed;
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

        bool touchingGround = TouchingGround();
        int move_dir = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);
        Vector2 newSpeed = new Vector2 (desiredSpeed, 0);
      //  desiredSpeed = _rb.linearVelocity.magnitude + acceleration * move_dir;
        //use magnitude instead of x to account for slopes
       //  desiredSpeed = Mathf.Clamp(newSpeed.x, -max_speed, max_speed);
        newSpeed = SetRealVelocity(move_dir);

        Debug.Log("Desired Speed is " + desiredSpeed);
        Debug.Log("New speed is " +  newSpeed);
        Debug.Log("_rb mag is " + _rb.linearVelocity.magnitude);
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

        bool isGrounded = IsGrounded();
        if (!isGrounded)
        {
            playerController.transform.rotation = Quaternion.identity;
        }
        animator.SetBool("IsGrounded", IsGrounded());

        if (playerInput.actions["Jump"].IsPressed())
        {
            jumpBuffer.StartTimer();
        }
        animator.SetFloat("JumpBuffer", jumpBuffer.timeRemaining());
        if (groundedLastFrame && !TouchingGround())
        {
            coyoteTimer.StartTimer();
        }

        if (move_dir == 0) { move_dir = Mathf.RoundToInt(playerController.transform.localScale.x); }
        playerController.gameObject.transform.localScale = new Vector2(move_dir, 1);
        //look in direction of movement


        setFacing();

        
    }

  
    Vector3 SetRealVelocity(float moveDir)
    {

        //Need to account for slopes
        RaycastHit2D hit = Physics2D.Raycast(playerController.transform.position, new Vector2(0, -1), GROUND_CHECKER_LENGTH * 2, groundMask);
        //Multiply ground checker length by 2 because downward slopes will make the raycast miss

        Vector3 slopeNormal = Vector3.Cross(hit.normal, new Vector2(moveDir, 0)) * desiredSpeed;
        //Use cross product to return normal of slope

        playerController.transform.rotation = Quaternion.LookRotation(slopeNormal);

        Debug.Log("Slope normal is " + slopeNormal);

        return slopeNormal * desiredSpeed;

    }


    public override bool IsGrounded()
    {
        if (!TouchingGround() && coyoteTimer.isStopped())
        {
            coyoteTimer.StartTimer(COYOTE_DURATION, false);
        }
        if (TouchingGround() || !coyoteTimer.isStopped())
        {
            return true;
        }
        return false;

    }

}
