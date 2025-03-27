using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using System.Collections;
using UnityEngine;


public class Player_SlamLand : Base_State
{
    Rigidbody2D _rb;

    [SerializeField] float _bonusSpeedMultiplierOnExit = 0.2f;
    [SerializeField] float _bounceAmount = 0.4f;
    [SerializeField] float _slamSpeed = 45.0f;

    HitboxComponent _hitbox;

    Transform spriteObject;

    const float SPRITE_SHAKE = 0.035f;
    const float CAMERA_SHAKE = 0.125f;
    Animator animator;

    bool shakePlayer = false;
    bool shakeCamera = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!stateInitalized)
        {
            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
            animator.updateMode = AnimatorUpdateMode.Fixed;
            playerInput = animator.gameObject.GetComponent<PlayerInput>();
            playerController = animator.gameObject.GetComponent<PlayerController>();
            _hitbox = playerController.GetHitbox();
            spriteObject = playerController.playerSprite.transform;
            this.animator = animator;
            InitInputActions(animator);
           stateInitalized = true;


            if (GameManager.instance != null)
            {
                shakeCamera = (bool) GameManager.instance.GetGameSetting("CameraShake");
            }
        }
        if (shakeCamera)
        {
            playerController.impulseSource.GenerateImpulse(CAMERA_SHAKE);
        }
        playerController.GetHitbox().hitboxConnected.AddListener(OnSlamCollision);
        animator.Play(layerIndex);

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (shakePlayer)
        {
            float shakeX = Random.Range(-SPRITE_SHAKE, SPRITE_SHAKE);            
            spriteObject.localPosition = new Vector2(shakeX, 0);
        }
    }

    void OnSlamCollision(GameObject obj, int stun, float dmg, Vector2 knockback)
    {
        if (!shakeCamera)
        {
            return;
        }
        if (obj.TryGetComponent<PlayerController>(out PlayerController player))
        {
            playerController.StartCoroutine(SetSlamHitshake(stun));
            Debug.Log("Hit player for knockback of " + knockback.magnitude.ToString());
            
                playerController.impulseSource.GenerateImpulse(CAMERA_SHAKE);
            
        }
    }

    public IEnumerator SetSlamHitshake(int stun)
    {
        float duration = stun / 2.0f;
        shakePlayer = true;
        animator.speed = 0.2f;
        yield return new WaitForSeconds(duration);

        EndHitshake();
    }

    void EndHitshake()
    {
        animator.speed = 1.0f;
        shakePlayer = false;
        spriteObject.localPosition = Vector2.zero;
    }
    
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x * (1 + (_bonusSpeedMultiplierOnExit * animator.GetInteger("HorizAxis"))), _slamSpeed * _bounceAmount);
        _hitbox.enabled = false;
    }

}
