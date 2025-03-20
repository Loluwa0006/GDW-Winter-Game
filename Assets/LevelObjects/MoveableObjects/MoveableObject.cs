using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] int _minStunTime = 12;
    [SerializeField] int _minDamage = 5;

   public UnityEvent DamagedEntity;

    Vector2 prevSpeed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {

        if (_rb == null)
        {
            Debug.Log("_rb is not configured in editor");
            _rb = GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate()
    {
        prevSpeed = _rb.linearVelocity;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_rb.linearVelocity.magnitude < _minSpeed)
        {
            return;
        }
     
        

        if (Vector2.Dot(collision.contacts[0].normal, _rb.linearVelocity.normalized)  > -0.05)
        {
            Debug.Log("was pushin");
            return;
        }
        HealthComponent healthComponent = collision.gameObject.GetComponent<HealthComponent>();
        float damage = 0.0f;

        if (healthComponent != null)
        {
            float health = healthComponent.GetHealth();
          
            damage = Mathf.Max(_rb.linearVelocity.magnitude * _rb.mass * _damageScaleFactor, _minDamage);
            Vector2 knockback = GetKnockBack(health, damage, collision.contacts[0].normal);

            healthComponent.Damage(knockback, damage);
            Rigidbody2D hitRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (hitRb != null)
            {
                hitRb.AddForce(knockback, ForceMode2D.Impulse);
            }

            DamagedEntity.Invoke();

        }
       
       

    }
  
    public Vector2 GetKnockBack(float health, float damage, Vector2 normal)
    {

        float knockbackForce = Mathf.Max(health * _knockbackScaleFactor + _baseKnockback, _baseKnockback);
        Vector2 knockback = knockbackForce * -normal;
        if (knockback.y < 3.0f)
        {
            knockback.y = 1.0f;
        }
        Debug.Log($"Knocking entity back {knockback}");
        return knockback;

        // Vector2 moveKnockback = Vector2.Max(_baseKnockback, _baseKnockback * (Vector2.one * (health * _knockbackScaleFactor)));
        //   Debug.Log("Knocking entity back " + moveKnockback.ToString());
        //   return moveKnockback;
    }
}
