using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static HitboxComponent;

[System.Serializable]
[RequireComponent(typeof(Timer))]
public class MoveableObject : MonoBehaviour
{

    public Rigidbody2D _rb;


    [SerializeField] float _knockbackScaleFactor = 0.7f;
    [SerializeField] float _damageScaleFactor = 0.015f;
    [SerializeField] float _baseKnockback = 30.0f;
    //saved as a float instead of a vector because normal determines knockback direction
    [SerializeField] float _minSpeed = 5.0f;
    [SerializeField] int _minDamage = 5;

   public UnityEvent DamagedEntity;

    PlayerController LastPlayerToPush;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {

        if (_rb == null)
        {
            Debug.Log("_rb is not configured in editor");
            _rb = GetComponent<Rigidbody2D>();
        }
    }

  


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_rb.linearVelocity.magnitude < _minSpeed)
        {
            return;
        }
     
        

        if (Vector2.Dot(collision.contacts[0].normal, _rb.linearVelocity.normalized)  > -0.05)
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController p))
            {
                LastPlayerToPush = p;
            }
            return;
        }
        HealthComponent healthComponent = collision.gameObject.GetComponent<HealthComponent>();
        float damage = 0.0f;

        if (healthComponent != null)
        {
            float health = healthComponent.GetHealth();
          
            damage = Mathf.Max(_rb.linearVelocity.magnitude * _rb.mass * _damageScaleFactor, _minDamage);
            Vector2 knockback = GetKnockBack(health, damage, collision.contacts[0].normal);

            HitboxComponent.HitboxInfo info = DetailedHitboxCollision(collision.gameObject, damage, knockback, collision.GetContact(0).point);
            healthComponent.Damage(info);

            Rigidbody2D hitRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (hitRb != null)
            {
                hitRb.AddForce(knockback, ForceMode2D.Impulse);
            }

            DamagedEntity.Invoke();

        }
       
       

    }

    public HitboxInfo DetailedHitboxCollision(GameObject collider, float damage, Vector2 push, Vector2 point)
    {
        HitboxInfo hitboxInfo = new HitboxInfo();
        hitboxInfo.push = push;
        hitboxInfo.point = point;
        hitboxInfo.damage = damage;
        hitboxInfo.hitObject = collider;
        hitboxInfo.hitboxOwner = LastPlayerToPush;

        return hitboxInfo;
    }

    public Vector2 GetKnockBack(float health, float damage, Vector2 normal)
    {

        float knockbackForce = Mathf.Max(health * _knockbackScaleFactor + _baseKnockback, _baseKnockback);
        Vector2 knockback = knockbackForce * -normal;
        if (knockback.y < 3.0f)
        {
            knockback.y = 1.0f;
            //no friction with the ground!
        }
        Debug.Log($"Knocking entity back {knockback}");
        return knockback;

        // Vector2 moveKnockback = Vector2.Max(_baseKnockback, _baseKnockback * (Vector2.one * (health * _knockbackScaleFactor)));
        //   Debug.Log("Knocking entity back " + moveKnockback.ToString());
        //   return moveKnockback;
    }

   
}
