using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(HealthComponent))]
[System.Serializable]
public class PlayerController : MonoBehaviour
{

    public UnityEvent hitboxEnabled = new UnityEvent();
    public UnityEvent hitboxDisabled = new UnityEvent();
    public UnityEvent<int> playerDead = new UnityEvent<int>();
    public UnityEvent<int> playerEliminated;
    //these exists to get around the inability of being able to connect statemachinebehavior functions to animation clips
    //because unity sucks uber omega butt cheeks

    [SerializeField] Animator animator;
    [SerializeField] BoxCollider2D hurtbox;
    [SerializeField] Rigidbody2D _rb;

    [SerializeField] Text stateTracker;
    [SerializeField] Text velocityTracker;
    [SerializeField] LayerMask groundMask;

    [SerializeField] HitboxComponent hitbox;

    public List<InputActionAsset> _playerKeybinds = new List<InputActionAsset>();

    public Transform _respawnPoint;

    Dictionary<string, Timer> _timerList = new Dictionary<string, Timer>();


    public PlayerInput _playerInput;




    public int playerIndex = 1;
    public int _remainingLives = 3;
    public enum GrapplePresets
    {
        REGULAR,
        SLINGSHOT,
        CHARGE
    }
    public GrapplePresets _currentGrapple = GrapplePresets.REGULAR;


    HealthComponent healthComponent;

    public Vector2 groundColliderSize;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        InitTimerList();
        _playerInput = GetComponent<PlayerInput>();
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.onEntityDamaged.AddListener(OnPlayerStruck);
        groundColliderSize = GetHurtbox().size * 0.8f;
        GetHitbox();

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
        this.playerIndex = playerIndex;
        _playerInput.actions = _playerKeybinds[playerIndex - 1];
        _playerInput.actions.Enable();
       // Debug.Log("enabled controls for player " + playerIndex.ToString());

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
            Debug.Log("Added timer " + timer.GetID());
        }
    }
    public void onPlayerDeath()
    {
        _remainingLives--;
        Debug.Log("dead");
        playerDead.Invoke(playerIndex);
        if (_remainingLives <= 0)
        {
            playerEliminated.Invoke(playerIndex);
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

        _rb.linearVelocity = Vector2.zero;

        healthComponent.Heal(Mathf.RoundToInt(healthComponent.getHealth()) + 1);

        playerDead.Invoke(_remainingLives);
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