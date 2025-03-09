using System;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_SlamLand : Base_State
{
    Rigidbody2D _rb;

    [SerializeField] float _bonusSpeedMultiplierOnExit = 0.2f;
    [SerializeField] float _bounceAmount = 0.4f;
    [SerializeField] float _slamSpeed = 45.0f;

    InputAction moveAsset;

    HitboxComponent _hitbox;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!stateInitalized)
        {
            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
            animator.updateMode = AnimatorUpdateMode.Fixed;
            playerInput = animator.gameObject.GetComponent<PlayerInput>();
            playerController = animator.gameObject.GetComponent<PlayerController>();
            _hitbox = playerController.GetHitbox();
            moveAsset = playerInput.actions["Move"];
           stateInitalized = true;
        }
        EndSlamMovement();
        

        animator.Play(layerIndex);

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
    

    public void EndSlamMovement()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x * (moveAsset.ReadValue<Vector2>().x * (1 + _bonusSpeedMultiplierOnExit)), _slamSpeed *  _bounceAmount);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _hitbox.gameObject.SetActive(false);
    }

}
