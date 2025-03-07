using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class AirState : Base_State
{
    [SerializeField] float jumpHeight = 5;
    [SerializeField] float jumpTimeToPeak = 0.4f;
    [SerializeField] float jumpTimeToDescent = 0.5f;
    [SerializeField] protected float acceleration = 0.35f;
    [SerializeField] protected float maxSpeed = 0.35f * 7f;
    [SerializeField] protected float maxFallSpeed = 25f;

    float jumpGravity;
    float fallGravity;
    protected float jumpVelocity;

    bool initalizedAirState = false;

    protected Rigidbody2D _rb;


    const float WALL_CHECKER_LENGTH = 0.45f;
    Timer jumpBuffer;

    LayerMask wallMask;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      

        useFixedUpdate = true;
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!initalizedAirState)
        {
            wallMask = LayerMask.GetMask("Wall");

            jumpBuffer = playerController.GetTimer("JumpBuffer");

          

            jumpVelocity = (2 * jumpHeight) / jumpTimeToPeak;
            jumpGravity = ((-2 * jumpHeight) / (jumpTimeToPeak * jumpTimeToPeak)) ;
            fallGravity = ((-2 * jumpHeight) / (jumpTimeToDescent * jumpTimeToDescent)) ;

            _rb = animator.GetComponentInParent<Rigidbody2D>();

            initalizedAirState =true;
        }
        playerController.transform.rotation = Quaternion.identity;
        _rb.freezeRotation = true;

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

        animator.SetInteger("HorizAxis", move_dir);

        bool crouch_held = playerInput.actions["Crouch"].IsPressed();
        animator.SetBool("CrouchHeld", crouch_held);

        bool touchingGround = TouchingGround();
        if (!touchingGround)
        {
            _rb.freezeRotation = false;
        }
        animator.SetBool("IsGrounded", touchingGround);
        animator.SetBool("MovingUpwards", (_rb.linearVelocity.y > 0));
        animator.SetBool("TouchingWall", TouchingWall());

        if (playerInput.actions["Jump"].WasPressedThisFrame())
        {
            jumpBuffer.StartTimer();
        }
        animator.SetFloat("JumpBuffer", jumpBuffer.timeRemaining());

        setFacing();
    }


    protected float getGravity()
    { 

        if (_rb.linearVelocity.y <= 0)
        {
            return fallGravity;
        }
        return jumpGravity;    
    }
    public bool TouchingWall()
    {
        float moveDir = playerInput.actions["Move"].ReadValue<Vector2>().x;
            if (moveDir == 0) { return false; } //need to be moving in direction of wall

        RaycastHit2D hit = Physics2D.BoxCast(playerController.transform.position, playerController.GetHurtbox().size, 0, new Vector2(moveDir, 0), WALL_CHECKER_LENGTH, wallMask);
        return hit;
    }
}
