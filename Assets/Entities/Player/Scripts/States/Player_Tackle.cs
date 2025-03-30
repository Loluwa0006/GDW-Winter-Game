using System.Collections;
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

    Transform spriteObject;

   const float    SPRITE_SHAKE = 0.045f;
    const float MIN_KNOCKBACK_FOR_CAMERA_SHAKE = 140f;
    const float CAMERA_SHAKE = 0.1f;
    Animator animator;

    bool shakePlayer = false;
    bool shakeCamera = false;

    

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.Play(layerIndex);

        if (!stateInitalized)
        {
            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
            
            this.animator = animator;

            if (GameManager.instance != null )
            {
                shakeCamera = (bool)GameManager.instance.GetGameSetting("CameraShake");
            }

        }

        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (spriteObject == null)
        {
            spriteObject = playerController.playerSprite.transform;
        }

       
            playerController.hitboxEnabled.AddListener(StartTackleMovement);
            playerController.hitboxDisabled.AddListener(EndTackleMovement);


        useGravity = false;

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0.0f);
        _tackleDirection = Mathf.RoundToInt(playerController.transform.localScale.x);

        animator.speed = 1.0f;

        playerController.GetHitbox().hitboxConnected.AddListener(OnTackleCollision);
        
    }

    void OnTackleCollision(HitboxComponent.HitboxInfo info)
    {
        Vector2 knockback = info.push;
        if (info.hitObject.TryGetComponent<PlayerController> (out PlayerController player))
        {
            playerController.StartCoroutine(SetTackleHitstopSpeed(info.stun));
            if (knockback.magnitude >= MIN_KNOCKBACK_FOR_CAMERA_SHAKE && shakeCamera)
            {
                playerController.impulseSource.GenerateImpulse(CAMERA_SHAKE);
            }
        }
    }

    IEnumerator SetTackleHitstopSpeed(int stun)
    {
        float duration = stun / 2.0f;
        Vector2 oldSpeed = _rb.linearVelocity;
        animator.speed = contactSpeed;
        _rb.linearVelocity *= contactSpeed;
        shakePlayer = true;

        yield return new WaitForSeconds(duration);

        EndHitshake(oldSpeed);
    }

    void EndHitshake(Vector2 oldSpeed)
    {
        _rb.linearVelocity = oldSpeed;
        animator.speed = 1.0f;
        shakePlayer = false;
        spriteObject.localPosition = Vector2.zero;
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

        if (shakePlayer)
        {
            float shakeX = Random.Range(-SPRITE_SHAKE, SPRITE_SHAKE);
            float shakeY = 0.0f;
            if (!IsGrounded()) 
            {
                shakeY = Random.Range(-SPRITE_SHAKE, SPRITE_SHAKE);

            }
            spriteObject.localPosition = new Vector2(shakeX, shakeY);
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
