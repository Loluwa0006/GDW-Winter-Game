using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class TetherPoint : MonoBehaviour
{
    const float SLINGSHOTMASSFACTOR = 1.15f;
    const float CHARGE_SPEED = 0.12f;
    const float MAX_CHARGE_AMOUNT = 2.75f;

    const float MIN_CHARGE_AMOUNT = 0.75f;

    const float TARGET_REACHED_DISTANCE = 1.0f;
    public Rigidbody2D _rb;
    public FixedJoint2D _joint;

    [SerializeField] float tetherSpeed = 50.0f;
    [SerializeField] float pullStrength = 20.0f;
    [SerializeField] float slingStrength = 300.0f;
    [SerializeField] Collider2D _collider;

    bool tetherLocked = false;

    Vector2 tetherPosition = Vector2.zero;


    [HideInInspector]
    public TetherPoint connectedTether = null;

    [SerializeField] LineRenderer lineRenderer;


    [SerializeField] Rigidbody2D connectedObject;

    PlayerController playerController;

    float tetherCharge = 0.0f;

    private void Awake()
    {
        lineRenderer.enabled = false;
    }

    public void FireTether(Vector2 dir, PlayerController player)
    {

        Physics2D.IgnoreCollision(_collider, player.GetHurtbox());
        playerController = player;
        _rb.linearVelocity = dir * tetherSpeed;

    }

    public void LinkToTether(TetherPoint tether)
    {
        connectedTether = tether;
        if (tether.connectedTether == null)
        {
            tether.connectedTether = this;
        }
        lineRenderer.enabled = true;
        DrawLine();

    }

    public void BreakLink()
    {
        connectedTether = null;
        lineRenderer.SetPosition(1, transform.position);
        lineRenderer.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (tetherLocked)
        {
            return;
        }
        tetherLocked = true;

        tetherPosition = collision.contacts[0].point;
        MoveableObject moveableObject = collision.collider.gameObject.GetComponent<MoveableObject>();
        Rigidbody2D collider_rb;

        if (moveableObject)
        {
            collider_rb = moveableObject._rb;
            moveableObject.DamagedEntity.AddListener(OnMoveableObjectCollision);
            //If the moveable object collides with a player, break the tether
        }
        else
        {
            collider_rb = collision.collider.gameObject.GetComponent<Rigidbody2D>();
        }
        Collider2D collider = collision.collider.gameObject.GetComponent<Collider2D>();
        if (collider_rb != null)
        {
            connectedObject = collider_rb;
            _joint.connectedBody = connectedObject;
            _joint.enabled = true;
            Debug.Log("Connecting to object " + collision.collider.name);
        }
        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0;
        if (connectedTether)
        {
            Physics2D.IgnoreCollision(collider, _collider);
            Physics2D.IgnoreCollision(connectedTether._collider, collider);
            //prevents the object from slamming into a tether instead of a player 
        }
    }

    void OnMoveableObjectCollision()
    {
        if (connectedTether != null)
        {
            Destroy(connectedTether.gameObject);
        }
        Destroy(gameObject);
    }
    private void FixedUpdate()
    {
        if (connectedTether)
        {
            DrawLine();
            if (connectedTether.tetherLocked)
            {
                PullObjects();
            }

            if (playerController.selectedTether == PlayerController.TetherPresets.CHARGE)
            {
                tetherCharge += CHARGE_SPEED;
            }

            //Debug.Log("Connected tether is " + connectedTether.gameObject.name);
        }

        if (tetherLocked && _rb.linearVelocity.magnitude > 0.0f)
        {
            Debug.Log("locked but still moving");
            _rb.linearVelocity = Vector2.zero;
        }

        if (connectedObject)
        {
            if (TargetReached()) Destroy(gameObject);
        }


    }

    void DrawLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        if (connectedTether)
        {
            lineRenderer.SetPosition(1, connectedTether.transform.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    bool BothTethersHaveSeperateObjects()
    {
        if (connectedTether.connectedObject == connectedObject)
        {
            Debug.Log("nope, connected to the same object");
            return false;
        }
        else if (!connectedObject && !connectedTether.connectedObject)
        {
            Debug.Log("nope, no objects");
            return false;
        }
        return true;
    }

    bool TargetReached()
    {
        if (connectedObject == null)
        {
            Debug.Log("no object");
            return true;
        }
        if (Vector2.Distance(connectedTether.transform.position, transform.position) <= TARGET_REACHED_DISTANCE)
        {
            Debug.Log("too close");
            return true;
        }
        return false;
    }
    void PullObjects()
    {
        if (!tetherLocked)
        {
            return;
        }

        Vector2 directionToTether = (connectedTether.transform.position - transform.position).normalized;
        switch (playerController.selectedTether)
        {
            case PlayerController.TetherPresets.CLASSIC:
                connectedObject.AddForce(directionToTether * pullStrength, ForceMode2D.Impulse);
                break;
            case PlayerController.TetherPresets.SLINGSHOT:
                 //connectedObject.linearVelocity = Vector2.zero;
                //reset it so that there's no force to counter act it
                //since it's only 1 time, it would suck without this
                //we also scale it with mass so that its not trash with heavier objects like anvils
                connectedObject.AddForce(directionToTether * slingStrength * ( 1 + connectedObject.mass * SLINGSHOTMASSFACTOR), ForceMode2D.Impulse);
                RemoveTether();
                break;

        }

    }
    // Update is called once per frame
    public void RemoveTether()
    {
        if (playerController.selectedTether == PlayerController.TetherPresets.CHARGE && connectedObject != null)
        {
            Vector2 directionToTether = (connectedTether.transform.position - connectedObject.transform.position).normalized;
            if (tetherCharge > MAX_CHARGE_AMOUNT)
            {
                tetherCharge = MAX_CHARGE_AMOUNT;
            }
            Vector2 force = directionToTether * slingStrength * (MIN_CHARGE_AMOUNT + tetherCharge) * SLINGSHOTMASSFACTOR;
           
            connectedObject.AddForce(force, ForceMode2D.Impulse);
        }
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (connectedTether != null)
        {
            Destroy(connectedTether.gameObject);
        }
    }
}