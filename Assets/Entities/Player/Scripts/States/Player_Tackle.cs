using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public class Player_Tackle : Base_State
{

    Rigidbody2D _rb;

    [SerializeField] float _tackleSpeed = 45.0f;
    [SerializeField] float verticalSpeedSlow = 0.85f;

    [SerializeField] float tackleGravity = 5.0f;

    bool useGravity = false;


    int _tackleDirection;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.Play(layerIndex);

        if (!stateInitalized)
        {
            _rb = animator.gameObject.GetComponent<Rigidbody2D>();

        }

        base.OnStateEnter(animator, stateInfo, layerIndex);

       
            playerController.hitboxEnabled.AddListener(StartTackleMovement);
            playerController.hitboxDisabled.AddListener(EndTackleMovement);


        useGravity = false;

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0.0f);
        _tackleDirection = Mathf.RoundToInt(playerController.transform.localScale.x);
    }

  

    public void StartTackleMovement()
    {
        _rb.linearVelocity = new Vector2(_tackleSpeed * _tackleDirection, _rb.linearVelocity.y * (1 - verticalSpeedSlow));
    }

    public void EndTackleMovement()
    {
        _rb.linearVelocity = Vector2.zero;
        useGravity = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (animator.GetInteger("HitstunAmount") <= 0)
        {
            if (!useGravity)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0.0f);
            }
            else
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -tackleGravity);
            }
        }


    }



    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _rb.linearVelocity = Vector2.zero;
        playerController.GetHitbox().enabled = false;

        playerController.hitboxEnabled.RemoveListener(StartTackleMovement);
        playerController.hitboxDisabled.RemoveListener(EndTackleMovement);


    }

}
