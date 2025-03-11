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
           stateInitalized = true;
        }
        EndSlamMovement(animator);
        

        animator.Play(layerIndex);

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }
    

    public void EndSlamMovement(Animator animator)
    {
        Debug.Log(_rb.linearVelocity.x * (1 + _bonusSpeedMultiplierOnExit));
        //Debug.Log(_rb.linearVelocity.x * (1 + _bonusSpeedMultiplierOnExit) * animator.GetInteger("HorizAxis"));
        
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x * (1 + (_bonusSpeedMultiplierOnExit * animator.GetInteger("HorizAxis"))), _slamSpeed *  _bounceAmount);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _hitbox.enabled = false;
    }

}
