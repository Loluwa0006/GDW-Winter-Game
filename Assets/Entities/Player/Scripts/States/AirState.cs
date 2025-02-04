using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class AirState : Base_State
{
    [SerializeField] float jumpHeight = 5;
    [SerializeField] float jumpTimeToPeak = 0.4f;
    [SerializeField] float jumpTimeToDescent = 0.5f;
    [SerializeField] float accleration = 0.35f;
    [SerializeField] float maxSpeed = 0.35f * 7f;
    [SerializeField] float maxFallSpeed = 25f;

    float jumpGravity;
    float fallGravity;
    protected float jumpVelocity;

    bool initalizedAirState = false;

    protected Rigidbody2D _rb;



    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        useFixedUpdate = true;
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (!initalizedAirState)
        {
            jumpVelocity = -2 * jumpHeight / jumpTimeToPeak;
            jumpGravity = -2 * jumpHeight / (jumpTimeToPeak * jumpTimeToPeak);
            fallGravity = -2 * jumpHeight / (jumpTimeToDescent * jumpTimeToDescent);

            _rb = animator.GetComponentInParent<Rigidbody2D>();

            initalizedAirState =true;
        }

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 newSpeed = _rb.linearVelocity;
        int move_dir = Mathf.RoundToInt(playerInput.actions["Move"].ReadValue<Vector2>().x);
        if (newSpeed.magnitude < maxSpeed || Mathf.Sign(newSpeed.x) != move_dir) 
        {
            newSpeed.x = _rb.linearVelocity.x + accleration * move_dir;

        }
        newSpeed.x = Mathf.Clamp(newSpeed.x, -maxSpeed, maxSpeed);

        newSpeed.y -= getGravity();
        if (newSpeed.y < maxFallSpeed)
        {
            newSpeed.y = maxFallSpeed;
        }

        _rb.linearVelocity = newSpeed * Time.deltaTime;

        animator.SetInteger("HorizAxis", move_dir);



        bool crouch_held = playerInput.actions["Crouch"].IsPressed();
        animator.SetBool("CrouchHeld", crouch_held);
        animator.SetBool("IsGrounded", playerController.isGrounded());
        animator.SetBool("movingUpwards", (_rb.linearVelocity.y > 0));
    }

    protected float getGravity()
    {
        if (_rb.linearVelocity.y < 0)
        {
            return fallGravity;
        }
        return jumpGravity;
    }
}
