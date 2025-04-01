using Unity.VisualScripting;
using UnityEngine;

public class MoveState : Base_State
{

    const float COYOTE_DURATION = 20f;
    const float DECEL_DAMPING = 10.5f;

    Rigidbody2D _rb;

    [SerializeField] float acceleration = 0.5f;
    [SerializeField] float max_speed = 2.5f;
    Timer jumpBuffer;
    Timer coyoteTimer;

    bool groundedLastFrame = false;

    float desiredSpeed;

    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (_rb == null)
        {
            jumpBuffer = playerController.GetTimer("JumpBuffer");
            coyoteTimer = playerController.GetTimer("Coyote");
            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
        }
     
    }



    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int move_dir = animator.GetInteger("HorizAxis");
        Vector2 newSpeed = _rb.linearVelocity;
   

        if (animator.GetInteger("HitstunAmount") <= 0)
        {
            if (newSpeed.magnitude < max_speed || Mathf.Sign(newSpeed.x) != move_dir)
            {
                _rb.AddForce(new Vector2(move_dir,0) * acceleration, ForceMode2D.Impulse);
            }
            if (move_dir == 0)
            {
                _rb.linearDamping = DECEL_DAMPING;
            }
            else
            {
                _rb.linearDamping = 0.0f;
            }
        }

        animator.SetBool("IsGrounded", IsGrounded());


        animator.SetFloat("JumpBuffer", jumpBuffer.TimeRemaining());
        if (groundedLastFrame && !TouchingGround())
        {
            coyoteTimer.StartTimer();
        }

        if (move_dir == 0) { move_dir = Mathf.RoundToInt(playerController.transform.localScale.x); }
        playerController.gameObject.transform.localScale = new Vector2(move_dir, 1);
        //look in direction of movement


        SetFacing();


    }


    Vector3 SetRealVelocity(float moveDir)
    {

        //Need to account for slopes
        RaycastHit2D hit = Physics2D.Raycast(playerController.transform.position, new Vector2(0, -1), GROUND_CHECKER_LENGTH * 2, playerController.groundMask);
        //Multiply ground checker length by 2 because downward slopes will make the raycast miss

        Vector3 slopeNormal = Vector3.Cross(hit.normal, new Vector2(moveDir, 0)) * desiredSpeed;
        //Use cross product to return normal of slope

        playerController.transform.rotation = Quaternion.LookRotation(slopeNormal);

        Debug.Log("Slope normal is " + slopeNormal);

        return slopeNormal * desiredSpeed;

    }


    public override bool IsGrounded()
    {
        if (!TouchingGround() && coyoteTimer.IsStopped())
        {
            coyoteTimer.StartTimer(COYOTE_DURATION, false);
        }
        if (TouchingGround() || !coyoteTimer.IsStopped())
        {
            return true;
        }
        return false;

    }


    protected override void InitInputActions(Animator animator)
    {
        playerInput.actions["Attack"].started += ctx => animator.SetTrigger("AttackPressed");
        playerInput.actions["Jump"].started += ctx => jumpBuffer.StartTimer();
        playerInput.actions["Move"].performed += ctx => SetMovementAxis(animator, ctx.ReadValue<Vector2>());
        playerInput.actions["Move"].canceled += ctx => SetMovementAxis(animator, Vector2.zero);



        //_ssControls.FindAction("Move").performed += ctx => animator.SetInteger("HorizAxis", Mathf.RoundToInt(ctx.ReadValue<Vector2>().x));
    }

    void SetMovementAxis(Animator animator, Vector2 move)
    {
        if (!animator) { return; }
        Vector2Int axis = Vector2Int.RoundToInt(move);

        animator.SetInteger("HorizAxis", axis.x);
        animator.SetInteger("VertAxis", axis.y);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {  

        if (_rb ==null)
        {
            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
        }
        _rb.linearDamping = 0.0f;
        
    }
}

  
