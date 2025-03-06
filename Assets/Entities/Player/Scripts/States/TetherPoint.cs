using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class TetherPoint : MonoBehaviour
{
    public Rigidbody2D _rb;
    public FixedJoint2D _joint;

    [SerializeField] float tetherSpeed = 50.0f;
    [SerializeField] float pullStrength = 20.0f;
    [SerializeField] float slingStrength = 300.0f;
    [SerializeField] Collider2D _collider;

    bool tetherLocked = false;

    Vector2 tetherPosition = Vector2.zero;

    public TetherPoint connectedTether = null;

    [SerializeField] LineRenderer lineRenderer;

   [SerializeField] Rigidbody2D connectedObject;




    PlayerController playerController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        //  healthComponent.onEntityMaxDamageReached.AddListener(breakLink);
    }

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
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, connectedTether.transform.position);
            if (connectedObject != null && connectedTether.tetherLocked)
            {
                pullObjects();
            }

            //Debug.Log("Connected tether is " + connectedTether.gameObject.name);
        }
        
       
    }

    void pullObjects()
    {
        if (connectedTether.connectedObject == connectedObject || !tetherLocked)
        {
            return;
        }
       
        Vector2 directionToTether = (connectedTether.transform.position - connectedObject.transform.position).normalized;
        switch (playerController._currentGrapple)
        {
            case PlayerController.GrapplePresets.REGULAR:
                connectedObject.AddForce(directionToTether * pullStrength); 
                break;
            case PlayerController.GrapplePresets.SLINGSHOT:
              //  connectedObject.linearVelocity = Vector2.zero;
                //reset it so that there's no force to counter act it
                //since it's only 1 time, it would suck without this
                connectedObject.AddForce(directionToTether * slingStrength);
                tetherLocked = false;
                break;
            case PlayerController.GrapplePresets.CHARGE:
                break; //to do later

        }

    }
  
    public bool tetherLinked() { return tetherLocked; }

    // Update is called once per frame
    void Update()
    {
        
    }
}
