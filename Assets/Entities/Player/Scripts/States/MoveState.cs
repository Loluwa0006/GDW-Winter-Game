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
        
            jumpBuffer = playerController.GetTimer("JumpBuffer");
        
      
            coyoteTimer = playerController.GetTimer("Coyote");
        
      

    }



    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int move_dir = animator.GetInteger("HorizAxis");
        Vector2 newSpeed = _rb.linearVelocity;
        newSpeed.x = _rb.linearVelocity.x + acceleration * move_dir;
        newSpeed.x = Mathf.Clamp(newSpeed.x, -max_speed, max_speed);

        if (animator.GetInteger("HitstunAmount") <= 0)
        {
            _rb.linearVelocity = newSpeed;
        }
 
        animator.SetBool("IsGrounded", IsGrounded());

       
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
        Vector2Int axis = Vector2Int.RoundToInt(move);

        animator.SetInteger("HorizAxis", axis.x);
        animator.SetInteger("VertAxis", axis.y);
    }
}
