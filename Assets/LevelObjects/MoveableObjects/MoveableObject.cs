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
    [SerializeField] float _minKnockback = 30.0f;
    [SerializeField] float _baseKnockback = 15.0f;
    [SerializeField] float _minSpeed = 5.0f;
    [SerializeField] int _minStunTime = 12;
    [SerializeField] int _minDamage = 5;
    [SerializeField] float _healthToStunScale = 0.0925f;
    [SerializeField] float _minRiseAmount = 5.0f;
    //when entity is hit they always go up by at least this amount

   public UnityEvent DamagedEntity; 


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
     
        HealthComponent health = collision.gameObject.GetComponent<HealthComponent>();
        float damage = 0.0f;
        int stunTime = 0;
        if (health != null)
        {
            damage = Mathf.Max( _rb.linearVelocity.magnitude * _rb.mass * _damageScaleFactor, _minDamage);
            stunTime = Mathf.RoundToInt(Mathf.Max(_minStunTime, (health.getHealth() * damage) * _healthToStunScale) );
            //health.Damage(GetKnockBack(health.getHealth(), damage),damage, stunTime);
            DamagedEntity.Invoke();
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 normal = collision.contacts[0].normal;
                Debug.Log("GLIZZLERS normal = " + normal.ToString());
                health.Damage(GetKnockBack(health.getHealth(), damage, normal), damage);
            }
        }
        else
        {
            Debug.Log("... no health detected");
        }
       

    }
  
    public Vector2 GetKnockBack(float health, float damage, Vector2 normal)
    {
        Vector2 knockback = Mathf.Max(health * _knockbackScaleFactor + _minKnockback, _minKnockback) * normal;
        knockback.y = Mathf.Max(_minRiseAmount, knockback.y);
        Debug.Log("Knocking entity back " + knockback.ToString());
        return knockback;
    }
}
