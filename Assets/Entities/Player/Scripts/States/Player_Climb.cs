using System.Net;
using UnityEngine;

public class Player_Climb : Base_State
{

   [SerializeField] LayerMask wallMask;

    [SerializeField] float climbSpeed;
    [SerializeField] float fallSpeed;
    [SerializeField] float fallAcceleration;
    [SerializeField] float climbAcceleration;
    [SerializeField] float latchSpeed;
    [SerializeField] float wallBounceDuration;
    [SerializeField] float mismatchedInputDetatchTimer;
    [SerializeField] float minWallBounceSpeed;

    float DetatchTracker = 0.1f;
    float wallBounceTracker = 0.0f;
    int wallDirection;
    Vector2 previousSpeed;
    Rigidbody2D _rb;

    Timer jumpBuffer;

    float WALL_CHECKER_LENGTH = 0.9f;

    int maxIterationsWithoutTouchingWall = 25;
    int iterationTracker = 0;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (_rb == null) {
            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
            jumpBuffer = playerController.GetTimer("JumpBuffer");
        }
        wallDirection = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);
        previousSpeed = _rb.linearVelocity;
        _rb.linearVelocity = Vector2.zero;

        wallBounceTracker = wallBounceDuration;
        DetatchTracker = mismatchedInputDetatchTimer;

        Debug.Log(wallDirection);

    }




    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

       
                animator.SetBool("TouchingWall", touchingWall());
            
        
       
        if (_rb.linearVelocityY < fallSpeed)
        {
            _rb.linearVelocityY *= latchSpeed;
        }


        if (playerInput.actions["Jump"].IsPressed())
        {
            jumpBuffer.StartTimer();
        }
        animator.SetFloat("JumpBuffer", jumpBuffer.timeRemaining());



        Vector2 move_dir = playerInput.actions["Move"].ReadValue<Vector2>();

        Vector2 newSpeed = _rb.linearVelocity;

        if (move_dir.y < 0)
        {
            newSpeed.y -= fallAcceleration;
            newSpeed.y = Mathf.Min(newSpeed.y, fallSpeed);
        }
        else if (move_dir.y > 0)
        {
            newSpeed.y += climbAcceleration;
            newSpeed.y = Mathf.Max(newSpeed.y, climbSpeed);
        }
        else if (animator.GetFloat("JumpBuffer") <= 0) //if the player is trying to leave the wall, stop resetting speed
        {
            newSpeed.y = 0;
        }

        if (Mathf.Sign(move_dir.x) != wallDirection || move_dir.x == 0)
        {
            if (DetatchTracker < 0)
            {
                animator.SetBool("TouchingWall", false);
            }
            else
            {
                DetatchTracker -= Time.deltaTime;
            }
        }
        else
        {
            DetatchTracker = mismatchedInputDetatchTimer;
        }

       
        if (wallBounceTracker > 0)
        {
            wallBounceTracker -= Time.deltaTime;
        }
        _rb.linearVelocity = newSpeed;
        
    }

    public bool touchingWall()
    {
        
        RaycastHit2D hit = Physics2D.Raycast(_rb.position, new Vector2(wallDirection, 0), WALL_CHECKER_LENGTH, wallMask);
        Vector2 endPoint = _rb.position + new Vector2(wallDirection, 0) * WALL_CHECKER_LENGTH;
        Color raycastColor = Color.red;
        if (hit)
        {
            raycastColor = Color.green;
        }
        Debug.DrawLine(_rb.position, endPoint, raycastColor, 5.0f);
        return hit;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 newSpeed = _rb.linearVelocity;
        if (wallBounceTracker > 0 && animator.GetFloat("JumpBuffer") > 0 && previousSpeed.magnitude > minWallBounceSpeed)
        {
            newSpeed.x = previousSpeed.x * -1;
            newSpeed.y = previousSpeed.y;
        }
        _rb.gravityScale = 1.0f;

    }

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
