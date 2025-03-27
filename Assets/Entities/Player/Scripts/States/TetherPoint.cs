using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class TetherPoint : MonoBehaviour
{
    const float SLINGSHOTMASSFACTOR = 4.5f;
    const float CHARGE_SPEED = 0.12f;
    const float MAX_CHARGE_AMOUNT = 2.75f;

    const float MIN_CHARGE_AMOUNT = 0.75f;
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


    [HideInInspector]
    [SerializeField] Rigidbody2D connectedObject;

    PlayerController playerController;

    float tetherCharge = 0.0f;
    

    public void FireTether(Vector2 dir, PlayerController player)
    {

        Physics2D.IgnoreCollision(_collider, player.GetHurtbox());
        playerController = player;
        _rb.linearVelocity = dir * tetherSpeed;

    }

    public void linkToTether(TetherPoint tether)
    {
        connectedTether = tether;
        if (tether.connectedTether == null)
        {
            tether.connectedTether = this;
        }
        DrawLine();

    }

    public void breakLink()
    {
        connectedTether = null;
        lineRenderer.SetPosition(1, transform.position);
        lineRenderer.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        tetherLocked = true;

        tetherPosition = collision.collider.transform.position;
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
        _rb.gravityScale = 0.0f;
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
        if (connectedTether != null)
        {
            DrawLine();
            if (connectedObject != null && connectedTether.tetherLocked)
            {
                pullObjects();
            }

            if (playerController.selectedTether == PlayerController.TetherPresets.CHARGE)
            {
                tetherCharge += CHARGE_SPEED;
            }

            //Debug.Log("Connected tether is " + connectedTether.gameObject.name);
        }


    }

    void DrawLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        if (connectedTether)
        {
            lineRenderer.SetPosition(1, connectedTether.transform.position);
        }
    }
    void pullObjects()
    {
        if (connectedTether.connectedObject == connectedObject || !tetherLocked)
        {
            return;
        }

        Vector2 directionToTether = (connectedTether.transform.position - connectedObject.transform.position).normalized;
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
                tetherLocked = false;
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