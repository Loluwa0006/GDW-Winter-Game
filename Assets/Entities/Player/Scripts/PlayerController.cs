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
   [SerializeField] Animator animator;
   [SerializeField] BoxCollider2D hurtbox;
   [SerializeField] Rigidbody2D _rb;

    [SerializeField] Text stateTracker;
    [SerializeField] Text velocityTracker;
    [SerializeField] LayerMask groundMask;
    [SerializeField] public  InputActionAsset _ssControls;

   public List<InputActionAsset> _playerKeybinds = new List<InputActionAsset>();

    public Transform _respawnPoint;

    Dictionary<string, Timer> _timerList = new Dictionary<string, Timer>();


   [SerializeField] PlayerInput _playerInput;

    UnityEvent<int> playerDead = new UnityEvent<int>();
    UnityEvent playerEliminated = new UnityEvent();


    public int playerIndex = 1;
    int _remainingLives = 3;
    public enum grappleElements
    {
        AIR,
        EARTH,
        FIRE,
        GRAVITY
    }
    public grappleElements grappleElement = grappleElements.AIR;



    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        InitTimerList();
        _playerInput = GetComponent<PlayerInput>();
      
    }



    public void EnablePlayer(int playerIndex)
    {
        this.playerIndex = playerIndex;
       
       _playerInput.actions = _playerKeybinds[playerIndex - 1];



        _playerInput.actions.Enable();

        Debug.Log("enabled controls for player " + playerIndex.ToString());

    }
    public void DisablePlayer()
    {
        _playerInput.actions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
      //  velocityTracker.text = "Velocity: " + _rb.linearVelocity.ToString(); 
        //   Debug.Log("Current state is now " + animator.GetCurrentAnimatorStateInfo(0).ToString());
    }

    public void UpdateStateTracker(string newState)
    {
        //stateTracker.text = "State: " + newState;
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
        Debug.Log("Couldn't find timer of name " + timerName);
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
            playerEliminated.Invoke();
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

        //below is deepseek code
        // Loop through all parameters
        ResetAnimatorParameters();
        //deepseek code over

        animator.SetBool("IsGrounded", false);

        _rb.linearVelocity = Vector2.zero;


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
