using UnityEngine;

public class Player_SlamStart : Base_State
{
    [SerializeField] float _movementDecreaseOnStart = 0.8f;
    Rigidbody2D _rb;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInitalized)
        {
            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
        }

        base.OnStateEnter(animator, stateInfo, layerIndex);

        animator.Play(layerIndex);
        StartSlamMovement();
    }

    public void StartSlamMovement()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x * (1 - _movementDecreaseOnStart), 0.0f);
    }

}
