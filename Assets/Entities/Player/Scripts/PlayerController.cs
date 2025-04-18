using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Unity.Cinemachine;

[RequireComponent(typeof(HealthComponent))]
[System.Serializable]
public class PlayerController : MonoBehaviour
{
    public GameObject playerSprite;

    [SerializeField] GameObject KOEffect;
    public const float RESPAWN_DELAY = 1.5f;
    [HideInInspector]
    public UnityEvent hitboxEnabled = new UnityEvent();
    [HideInInspector]
    public UnityEvent hitboxDisabled = new UnityEvent();
    //these exists to get around the inability of being able to connect statemachinebehavior functions to animation clips
    //because unity sucks uber omega butt cheeks

    [HideInInspector]
    public UnityEvent<PlayerController> playerEliminated;


    [HideInInspector]
    public UnityEvent<PlayerController> playerRespawned = new();

    [SerializeField] Animator animator;
    [SerializeField] BoxCollider2D hurtbox;
    [SerializeField] Rigidbody2D _rb;
    public CinemachineImpulseSource impulseSource;

    public GameObject _grapplePrefab;
    [HideInInspector]
    public GameObject activeGrapple;
    public LayerMask groundMask;

    [SerializeField] HitboxComponent hitbox;

    public List<InputActionAsset> _playerKeybinds = new List<InputActionAsset>();

    public Transform _respawnPoint;

    Dictionary<string, Timer> _timerList = new Dictionary<string, Timer>();


    public PlayerInput _playerInput;



    public int playerIndex = 1;
    [HideInInspector]
    public enum TetherPresets
    {
        CLASSIC,
        SLINGSHOT,
        CHARGE
    }
    public TetherPresets selectedTether = TetherPresets.CLASSIC;


    HealthComponent healthComponent;

    public Vector2 groundColliderSize;

     bool droppingThroughPlatform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    LayerMask platformMask;

    public List<Color> playerColors = new();

    public bool deadAF = false;

    public AudioSource playerAudio;


    private void Awake()
    {
        InitHealthComponent();
        groundColliderSize = GetHurtbox().size * 0.8f;
        GetHitbox();

        platformMask = LayerMask.GetMask("Platform");

        PhysicsMaterial2D material2D = new PhysicsMaterial2D();

        material2D.bounciness = _rb.sharedMaterial.bounciness;
        material2D.friction = _rb.sharedMaterial.friction;

        _rb.sharedMaterial = material2D;
        InitTimerList();

        DontDestroyOnLoad(this);

    }

    void InitHealthComponent()
    {
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.onPlayerInjured.AddListener(OnPlayerStruck);
    }

    void InitUniversalInputActions()
    {
        _playerInput.actions["DropDown"].performed += ctx => StartCoroutine(DropThroughPlatform());
    }

    private void Start()
    {
    }

    private IEnumerator DropThroughPlatform()
    {

        Debug.Log("Droppin ");

        droppingThroughPlatform = true;
        _rb.excludeLayers = platformMask;
        hurtbox.excludeLayers = platformMask;

        yield return new WaitForSeconds(0.65f);
        droppingThroughPlatform = false;
        Debug.Log("stoppin ");
        _rb.excludeLayers = 0;
        hurtbox.excludeLayers = 0;
    }


    private void Update()
    {
       if (_rb.linearVelocity.y > 0)
        {
            _rb.excludeLayers = platformMask;
            hurtbox.excludeLayers = platformMask;
        }
       else if (!droppingThroughPlatform) 
        {
            _rb.excludeLayers = 0;
            hurtbox.excludeLayers = 0;
        }
    }
    public void OnHitboxEnabled()
    {
        hitboxEnabled.Invoke();
    }

    public void OnHitboxDisabled()
    {
        hitboxDisabled.Invoke();
    }

    void OnPlayerStruck(HitboxComponent.HitboxInfo info)
    {
        animator.SetBool("GrappleActive", false);
        if (activeGrapple)
        {
            Destroy(activeGrapple);
        }
     
        animator.SetInteger("HitstunAmount", info.stun);
        animator.SetFloat("HitshakeAmount", info.shake);
        animator.SetFloat("KnockbackX", info.push.x);
        animator.SetFloat("KnockbackY", info.push.y);
        animator.SetFloat("HitboxCollisionX", info.point.x);
        animator.SetFloat("HitboxCollisionY", info.point.y);
    }

    public HitboxComponent GetHitbox()
    {
        return hitbox;
    }

    public void EnablePlayer(int playerIndex)
    {
        this.playerIndex = playerIndex;
        _playerInput.actions = _playerKeybinds[playerIndex - 1];
        _playerInput.actions.Enable();
        _playerInput.SwitchCurrentActionMap("BattleControls");    
        
        if (playerIndex != 1 && Gamepad.all.Count > 0)
        {
            _playerInput.SwitchCurrentControlScheme(Gamepad.all[playerIndex - 2]);
        }
        
        healthComponent.playerIndex = playerIndex;

        if (GameManager.instance != null)
        {
            selectedTether = GameManager.instance.GetPlayerTether(playerIndex - 1);
            playerSprite.GetComponent<SpriteRenderer>().color = GameManager.instance.playerColors[playerIndex - 1];

        }
        else
        {
            playerSprite.GetComponent<SpriteRenderer>().color = playerColors[playerIndex - 1];
        }

        InitUniversalInputActions();

    }
    public void DisablePlayer()
    {
        _playerInput.actions.Disable();
    }


    public int HasParameter(string parameterName)
    {
        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.parameters[i].name == parameterName)
            {
                return i;
            }
        }
        return -1;
    }

    public BoxCollider2D GetHurtbox()
    {
        return hurtbox;
    }

    public Timer GetTimer(string timerName)
    {
        if (_timerList.ContainsKey(timerName))
        {
            return _timerList[timerName];
        }
        Debug.LogWarning("Couldn't find timer of name " + timerName);
        return null;
    }

    void InitTimerList()
    {
        foreach (Component component in GetComponentsInChildren<Timer>())
        {
            Timer timer = (Timer)component;
            _timerList.Add(timer.GetID(), timer);
            DontDestroyOnLoad(timer);
            DontDestroyOnLoad(timer.gameObject);
        }
    }
    public void OnPlayerDeath()
    {
        PlayKoEffect();
        deadAF = true;
      
        hurtbox.enabled = false;
        if (healthComponent.GetRemainingLives() - 1 <= 0)
        {
            playerEliminated.Invoke(this);
        }
        else
        {
            StartCoroutine( Respawn());
        }   
    }

    void PlayKoEffect()
    {
        GameObject KOeffect = Instantiate(KOEffect);
        KOeffect.GetComponent<SpriteRenderer>().color = playerColors[playerIndex - 1];
        Vector3 offset = (_rb.linearVelocity.normalized) * 10.0f;
        KOeffect.transform.position = transform.position - offset;

        float angle = Mathf.Atan2(-_rb.linearVelocity.y, -_rb.linearVelocity.x) * Mathf.Rad2Deg;
        KOeffect.transform.rotation = Quaternion.Euler(0, 0, angle - 45);

        KOeffect.GetComponent<SpriteRenderer>().color = playerColors[playerIndex - 1];
    }
    IEnumerator Respawn()
    {
      
        healthComponent.RemoveLife();
        if (activeGrapple)
        {
            activeGrapple.SetActive(false);
        }

        yield return new WaitForSeconds(RESPAWN_DELAY);
        transform.position = _respawnPoint.position;

        ResetAnimatorParameters();

        animator.SetBool("IsGrounded", false);

        ResetRigidBody();

        playerRespawned.Invoke(this);
        yield return new WaitForEndOfFrame();
        //im just hitting buttons on my keyboard tryna fix this problem im ngl
        hurtbox.enabled = true;
        deadAF = false;

    }

    void ResetRigidBody()
    {
        _rb.linearVelocity = Vector2.zero;

        _rb.sharedMaterial.bounciness = 0.0f;
        _rb.sharedMaterial.friction = 0.0f;

        _rb.gravityScale = 1.0f;
    }

    void ResetAnimatorParameters()
    {
        //below is deepseek code
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(param.name, 0f); // Reset float to 0
                    break;

                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(param.name, 0); // Reset int to 0
                    break;

                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(param.name, false); // Reset bool to false
                    break;

                case AnimatorControllerParameterType.Trigger:
                    animator.ResetTrigger(param.name); // Reset trigger
                    break;
            }

        }
        //deepseek code over

    }

    public static float GetRespawnDelay()
    {
        return RESPAWN_DELAY;
    }


}