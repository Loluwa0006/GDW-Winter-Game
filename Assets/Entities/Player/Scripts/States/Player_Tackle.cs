using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public class Player_Tackle : Base_State
{

    Rigidbody2D _rb;

    [SerializeField] float _tackleSpeed = 45.0f;


    int _tackleDirection;

    bool _hitboxEventsConnected = false;
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
            playerController.hitboxEnabled.AddListener(StartTackleMovement);
            playerController.hitboxDisabled.AddListener(EndTackleMovement);

        }



        _tackleDirection = Mathf.RoundToInt(playerController.transform.localScale.x);
        animator.Play(layerIndex);
    }

  

    public void StartTackleMovement()
    {
        _rb.linearVelocity = new Vector2(_tackleSpeed * _tackleDirection, 0.0f);
    }

    public void EndTackleMovement()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (animator.GetInteger("HitstunAmount") <= 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0.0f);
        }


    }



    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _rb.linearVelocity = Vector2.zero;
        playerController.GetHitbox().gameObject.SetActive(false);
    }

}
