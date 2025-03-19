using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(BoxCollider2D))]
[System.Serializable]
public class HitboxComponent : MonoBehaviour
{


    public BoxCollider2D hitboxInfo;

    public int damage = 2;
    public int stunTime = 15;

    //if stun time is not 0, then hitstun is fixed
    public float _knockbackScaleFactor;
    public Vector2 _baseKnockback = new Vector2(20, 50);

    //for animator purposes, reset this value so attacks can land
    public bool attackLanded = false;

    public UnityEvent hitboxConnected = new UnityEvent();


    private void Awake()
    {

        Collider2D parentHurtbox = transform.parent.GetComponent<Collider2D>();
        if (parentHurtbox != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), parentHurtbox);
            //makes it so that you can't hit yourself
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {

        HealthComponent health = collision.GetComponent<HealthComponent>();
        Vector2 push = Vector2.zero;
        if (health != null)
        {
          push = GetKnockBack(health.GetHealth(), damage);
          health.Damage(push, damage);

        }
        else
        {
            Debug.Log("Couldn't get health component");
        }
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
        {
            //Assume to the right of the object
            //need to launch rigid body in opposite direction of where im facing, 
            if (transform.position.x > collision.transform.position.x)
            //if you're to the left of the object, push the other way
            {
                push.x *= -1;
            }
            
            rb.AddForce(push);
            Debug.Log("Hit " + collision.attachedRigidbody.gameObject.name);
        }
        hitboxConnected.Invoke();
        enabled = false;
    }


    public Vector2 GetKnockBack(float health, float damage)
    {
        Vector2 moveKnockback = Vector2.Max(_baseKnockback, _baseKnockback *  (Vector2.one * (health * _knockbackScaleFactor)));
        Debug.Log("Knocking entity back " + moveKnockback.ToString());
        return moveKnockback;
    }


  
}