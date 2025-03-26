using NUnit.Framework.Constraints;
using System.Collections;
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

    const float contactSpeed = 0.35f;

    Animator animator;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.Play(layerIndex);

        if (!stateInitalized)
        {
            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
            this.animator = animator;

        }

        base.OnStateEnter(animator, stateInfo, layerIndex);

       
            playerController.hitboxEnabled.AddListener(StartTackleMovement);
            playerController.hitboxDisabled.AddListener(EndTackleMovement);


        useGravity = false;

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0.0f);
        _tackleDirection = Mathf.RoundToInt(playerController.transform.localScale.x);

        animator.speed = 1.0f;

        playerController.GetHitbox().hitboxConnected.AddListener(OnTackleCollision);
        
    }

    void OnTackleCollision(GameObject obj, int stun)
    {
        if (obj.TryGetComponent<PlayerController> (out PlayerController player))
        {
            playerController.StartCoroutine(SetTackleHitstopSpeed(stun));
        }
    }

    IEnumerator SetTackleHitstopSpeed(int stun)
    {
        float duration = stun / 1.5f;
        Debug.Log("Slowing down " + playerController.playerIndex + " to speed " + contactSpeed + " for " + duration.ToString());
        Vector2 oldSpeed = _rb.linearVelocity;
        animator.speed = contactSpeed;
        _rb.linearVelocity *= contactSpeed;
        yield return new WaitForSeconds(duration);
        _rb.linearVelocity = oldSpeed;
        animator.speed = 1.0f;
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
        animator.SetBool("IsGrounded", TouchingGround());
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
        RaycastHit2D hit = Physics2D.Raycast(spawnPos, new Vector2(0, -1), SLAM_CHECKER_LENGTH, playerController.groundMask);
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
