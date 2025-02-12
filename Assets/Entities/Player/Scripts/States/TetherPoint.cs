using Unity.VisualScripting;
using UnityEngine;


[System.Serializable]
public class TetherPoint : MonoBehaviour
{
    public Rigidbody2D _rb;

    [SerializeField] float tetherSpeed = 50.0f;
    [SerializeField] float pullStrength = 20.0f;

    bool tetherLocked = false;

    Vector2 tetherPosition = Vector2.zero;

    public TetherPoint connectedTether = null;

    [SerializeField] LineRenderer lineRenderer;

    Rigidbody2D connectedObject;



    // Start is called once before the first execution of Update after the MonoBehaviour is created

  
    public void FireTether(Vector2 dir)
    {
        
    
        _rb.linearVelocity = dir.normalized * tetherSpeed;
   
    }

    public void linkToTether(TetherPoint tether)
    {
        Debug.Log("LINKING");
        connectedTether = tether;
        Debug.Log("LINK SUCCESSFUL");
    }

    public void breakLink()
    {
        connectedTether = null;
        connectedObject = null;
        lineRenderer.SetPosition(1, transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        tetherLocked = true;

        tetherPosition = collision.collider.transform.position;
        Rigidbody2D collider_rb = collision.collider.gameObject.GetComponent<Rigidbody2D>();
        if (collider_rb != null)
        {
            connectedObject = collider_rb;
        }
        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 0.0f;
    }

    private void FixedUpdate()
    {
        if (connectedTether != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, connectedTether.transform.position);

            //Debug.Log("Connected tether is " + connectedTether.gameObject.name);
        }
        if (connectedObject != null)
        {
            connectedObject.AddForce(Vector2.one * pullStrength);
        }
       
    }

  
    public bool tetherLinked() { return tetherLocked; }

    // Update is called once per frame
    void Update()
    {
        
    }
}
