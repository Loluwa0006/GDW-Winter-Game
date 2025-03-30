using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(BoxCollider2D))]
[System.Serializable]
public class HitboxComponent : MonoBehaviour
{
    const float HEALTH_SCALE = 0.85f;

    public BoxCollider2D hitboxInfo;

    public int damage = 2;
    public int stunTime = 15;

    public float _knockbackScaleFactor;
    public Vector2 _baseKnockback = new Vector2(20, 50);

    //for animator purposes, reset this value so attacks can land
    public bool attackLanded = false;

    public class HitboxInfo
    {
        public GameObject hitObject;
        public float damage;
        public Vector2 push;
        public Vector2 point;
        public int stun;
        //collision point;
        public float shake;
    }

    public UnityEvent <HitboxInfo> hitboxConnected = new();

    Collider2D hitbox;
    Collider2D parentHurtbox;
    private void Awake()
    {
        hitbox = GetComponent<Collider2D>();
        parentHurtbox = transform.parent.GetComponent<Collider2D>();
        if (parentHurtbox != null)
        {
            Physics2D.IgnoreCollision(hitbox, parentHurtbox);
            //makes it so that you can't hit yourself
        }
    }

    private void OnEnable()
    {
        
        Collider2D[] objects = Physics2D.OverlapBoxAll(transform.position, hitbox.bounds.size, 0);

        foreach (Collider2D obj in objects)
        {
            if (obj != null)
            {
                if (parentHurtbox)
                {
                    if (obj == parentHurtbox) return;
                }
                HandleHit(obj);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleHit(collision);
    }

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void HandleHit(Collider2D collision)
    {

        HealthComponent health = collision.GetComponent<HealthComponent>();
        
        HitboxInfo info = DetailedHitboxCollision(collision.gameObject, damage, GetKnockBack(health), collision.ClosestPoint(transform.position));

        if (transform.position.x > collision.transform.position.x)
        //if you're to the left of the object, push the other way
        {
            info.push.x *= -1;
        }

        if (health != null)
        {
           health.Damage(info);
        }

        else
        {
            Debug.Log("Couldn't get health component");

            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb != null)
            {
                rb.AddForce(info.push, ForceMode2D.Impulse);
                Debug.Log("Hit " + collision.attachedRigidbody.gameObject.name + " with push force of " + info.push.ToString()) ;
            }
        }
            hitboxConnected.Invoke(info);
            enabled = false;
        }

    public static HitboxInfo DetailedHitboxCollision(GameObject collider, float damage, Vector2 push, Vector2 point)
    {
        HitboxInfo hitboxInfo = new HitboxInfo();
        hitboxInfo.push = push;
        hitboxInfo.point = point;
        hitboxInfo.damage = damage;
        hitboxInfo.hitObject = collider;

        return hitboxInfo;
    }

    public Vector2 GetKnockBack(HealthComponent hp)
    {
        if (!hp)
        {
            return _baseKnockback;
        }
        float health = hp.GetHealth();
        Vector2 moveKnockback = Vector2.Max(_baseKnockback, _baseKnockback *  (Vector2.one * (health * HEALTH_SCALE) * _knockbackScaleFactor));
        Debug.Log("Knocking entity back " + moveKnockback.ToString());
        return moveKnockback;
    }


  
}