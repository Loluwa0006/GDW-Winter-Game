using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(BoxCollider2D))]
[System.Serializable]
public class HitboxComponent : MonoBehaviour
{


    public BoxCollider2D hitboxInfo;

    [SerializeField] int damage = 2;
    [SerializeField] int stunTime = 15;

    //if stun time is not 0, then hitstun is fixed
    [SerializeField] float _knockbackScaleFactor;
    [SerializeField] Vector2 _baseKnockback = new Vector2(20, 50);

    //for animator purposes, reset this value so attacks can land
    [SerializeField] bool attackLanded = false;

    public UnityEvent hitboxConnected = new UnityEvent();


    private void Awake()
    {

        Collider2D parentHurtbox = transform.parent.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), parentHurtbox);
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {


        HealthComponent health = collision.GetComponent<HealthComponent>();
        if (health != null)
        {
            if (stunTime > 0)
            {
                health.Damage(damage, stunTime);
            }
            else
            {
                health.Damage(damage);
            }
        }
        else
        {
            Debug.Log("Couldn't get health component");
        }
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
        {
            //Assume to the right of the object
            Vector2 push = GetKnockBack(health.getHealth(), damage);
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
        Vector2 moveKnockback = _baseKnockback + (Vector2.one * (health * _knockbackScaleFactor));
        Debug.Log("Knocking entity back " + moveKnockback.ToString());
        return moveKnockback;
    }


    // Update is called once per frame
    void Update()
    {

    }
}