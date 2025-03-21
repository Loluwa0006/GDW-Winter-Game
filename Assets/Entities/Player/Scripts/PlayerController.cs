using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(HealthComponent))]
[System.Serializable]
public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent hitboxEnabled = new UnityEvent();
    [HideInInspector]
    public UnityEvent hitboxDisabled = new UnityEvent();
    [HideInInspector]
    public UnityEvent<PlayerController> playerEliminated;
    [HideInInspector]
    //these exists to get around the inability of being able to connect statemachinebehavior functions to animation clips
    //because unity sucks uber omega butt cheeks

    [SerializeField] Animator animator;
    [SerializeField] BoxCollider2D hurtbox;
    [SerializeField] Rigidbody2D _rb;


    public GameObject _grapplePrefab;
    [HideInInspector]
    public GameObject _activeGrapple;
    public LayerMask groundMask;

    [SerializeField] HitboxComponent hitbox;

    public List<InputActionAsset> _playerKeybinds = new List<InputActionAsset>();

    public Transform _respawnPoint;

    Dictionary<string, Timer> _timerList = new Dictionary<string, Timer>();


    public PlayerInput _playerInput;



    [HideInInspector]
    public int playerIndex = 1;
    [HideInInspector]
    public enum GrapplePresets
    {
        REGULAR,
        SLINGSHOT,
        CHARGE
    }
    public GrapplePresets _currentGrapple = GrapplePresets.REGULAR;


    HealthComponent healthComponent;

    public Vector2 groundColliderSize;

     bool droppingThroughPlatform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    LayerMask platformMask;

    private void Awake()
    {
        InitHealthComponent();
        groundColliderSize = GetHurtbox().size * 0.8f;
        GetHitbox();

        InitUniversalInputActions();
        platformMask = LayerMask.GetMask("Platform");

        PhysicsMaterial2D material2D = new PhysicsMaterial2D();

        material2D.bounciness = _rb.sharedMaterial.bounciness;
        material2D.friction = _rb.sharedMaterial.friction;

        _rb.sharedMaterial = material2D;
    }

    void InitHealthComponent()
    {
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.onEntityDamaged.AddListener(OnPlayerStruck);
    }

    void InitUniversalInputActions()
    {
        _playerInput.actions["DropDown"].performed += ctx => StartCoroutine(DropThroughPlatform());

    }

    private void Start()
    {
        InitTimerList();
    }

    private IEnumerator DropThroughPlatform()
    {
        droppingThroughPlatform = true;
        _rb.excludeLayers = platformMask;
        hurtbox.excludeLayers = platformMask;

        yield return new WaitForSeconds(0.5f);
        _rb.excludeLayers = 0;
        hurtbox.excludeLayers = 0;
        droppingThroughPlatform = false;
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

    void OnPlayerStruck(float damageTaken, int stunTime)
    {
        animator.SetInteger("HitstunAmount", stunTime);
    }

    public HitboxComponent GetHitbox()
    {
        return hitbox;
    }

    public void EnablePlayer(int playerIndex)
    {
        Debug.Log("player index is " + playerIndex.ToString());
        this.playerIndex = playerIndex;
        _playerInput.actions = _playerKeybinds[playerIndex - 1];
        _playerInput.actions.Enable();
        _playerInput.SwitchCurrentActionMap("BattleControls");    
       // _playerInput.SwitchCurrentControlScheme("KeyboardMouse", Keyboard.current);
        Debug.Log("Enabled actions for player " + playerIndex.ToString());
        // Debug.Log("enabled controls for player " + playerIndex.ToString());
       
        healthComponent.playerIndex = playerIndex;

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
          //  Debug.Log("Added timer " + timer.GetID());
        }
    }
    public void OnPlayerDeath()
    {
        int lives = healthComponent.GetRemainingLives() - 1;
        if (lives <= 0)
        {
            playerEliminated.Invoke(this);
            Destroy(gameObject);
        }
        else
        {
            Respawn();
        }
    }
    void Respawn()
    {

        transform.position = _respawnPoint.position;

        ResetAnimatorParameters();

        animator.SetBool("IsGrounded", false);

        healthComponent.RemoveLife();

        
        Destroy(_activeGrapple);

        ResetRigidBody();
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


}