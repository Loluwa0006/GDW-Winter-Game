using UnityEngine;
using UnityEngine.Animations;

public class Player_Slam : Base_State
{

    Rigidbody2D _rb;

    [SerializeField] float _slamSpeed = 45.0f;
    [SerializeField] float _movementDecreaseOnStart = 0.8f;
    [SerializeField] float _bonusSpeedMultiplierOnExit = 0.2f;
    [SerializeField] float _slamTurnSpeed = 0.2f;
    [SerializeField] float _bounceAmount = 0.4f;
    private bool _hitboxEventsConnected;

    float moveDir;

    enum SlamState
    {
        CHARGING,
        FALLING,
        LANDING
    }

    SlamState _currentState = SlamState.CHARGING;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInitalized)
        {
            _rb = animator.gameObject.GetComponent<Rigidbody2D>();

        }

        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (!_hitboxEventsConnected)
        {
            _hitboxEventsConnected = true;
            playerController.hitboxEnabled.AddListener(StartSlamMovement);
            playerController.hitboxDisabled.AddListener(EndSlamMovement);

        }

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0.0f);


        animator.Play(layerIndex);
    }


    void ChangeSlamState(SlamState newState)
    {
        _currentState = newState;

        switch (_currentState)
        {
            case SlamState.FALLING:
                StartSlamMovement();
                break;
            case SlamState.LANDING:
                EndSlamMovement();
                break;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex, controller);

        if (_currentState == SlamState.FALLING)
        {
            if (IsGrounded())
            {
                ChangeSlamState(SlamState.LANDING);
            }
        }
    }

    public void StartSlamMovement()
    {
        _rb.linearVelocity = new Vector2( _rb.linearVelocity.x * (1 - _movementDecreaseOnStart), _slamSpeed);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        moveDir = animator.GetInteger("HorizAxis");
    }
    public void EndSlamMovement()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x * (moveDir * (1 + _bonusSpeedMultiplierOnExit)), _bounceAmount);
    }



    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        playerController.GetHitbox().enabled = false;
    }

}
