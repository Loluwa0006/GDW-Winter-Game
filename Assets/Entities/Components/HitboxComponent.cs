using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
[System.Serializable]
public class HitboxComponent : MonoBehaviour
{

    public BoxCollider2D hitboxInfo;

    [SerializeField] int damage = 2;
    [SerializeField] bool hasKnockback = false;
    [SerializeField] bool hasStun = false;
    [SerializeField] Vector2 attackPush;
    

    //for animator purposes, reset this value so attacks can land
    [SerializeField] bool attackLanded = false;

    enum attackTypes
    {
        STRIKE, 
        PROJECTILE,
        GRAB
    }
    attackTypes attackType = attackTypes.STRIKE;


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (attackLanded)
        {
            return;
//can only hit with attacks once;
        }
        else if (collision.gameObject == transform.parent.gameObject)
        {
            return;
            //can't hit yourself
        }
        HealthComponent health = collision.GetComponent<HealthComponent>();
        if (health != null)
        {
            health.Damage(damage);
        }
        Rigidbody2D rb = collision.attachedRigidbody;
        if (rb != null)
        {
            //Assume to the right of the object
            Vector2 push = new Vector2(attackPush.x * -1, attackPush.y);
                //need to launch rigid body in opposite direction of where im facing, 
            if (transform.position.x < collision.transform.position.x)
                //if you're to the left of the object, push the other way
            {
                push.x *= -1;
            }
            rb.AddForce(push);
            Debug.Log("Hit " + collision.attachedRigidbody.gameObject.name);
        }
        attackLanded = true;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
