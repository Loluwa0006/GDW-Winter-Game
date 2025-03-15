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

    const float SLAM_CHECKER_LENGTH = 2.0f;


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
        animator.SetBool("CanSlam", CanSlam());
        



    }



    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _rb.linearVelocity = Vector2.zero;
        playerController.GetHitbox().enabled = false;

        playerController.hitboxEnabled.RemoveListener(StartTackleMovement);
        playerController.hitboxDisabled.RemoveListener(EndTackleMovement);


    }

    public bool CanSlam()
    {
        Vector2 spawnPos = new Vector2(_rb.position.x, _rb.position.y - playerController.groundColliderSize.y);
        RaycastHit2D hit = Physics2D.Raycast(spawnPos, new Vector2(0, -1), SLAM_CHECKER_LENGTH, groundMask);
        Color rayColor = Color.red;
        bool valToReturn = false;
        if (hit)
        {
            rayColor = Color.green;

            valToReturn = false;
        }
        else
        {
            valToReturn = true;
        }


        Debug.DrawRay(spawnPos, new Vector2(0, -1), rayColor);
        return valToReturn;
        //invert it, because if its colliding, then the player is too close to the ground 
    }

}
