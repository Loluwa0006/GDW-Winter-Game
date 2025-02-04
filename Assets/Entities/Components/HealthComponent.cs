using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] int max_health = 100;
    int health = 0;
    public UnityEvent onEntityDamaged;
    public UnityEvent onEntityHealed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = max_health;
    }

    public virtual void Damage(int damage_taken = 1)
    {
        health -= damage_taken;
        if (health < 0)
        {
            onEntityDead();
        }

        onEntityDamaged.Invoke();
        
    }

    public virtual void Heal(int heal_amount = 1)
    {
        health += heal_amount;
        health = Mathf.Clamp(health, 0, max_health);
        onEntityHealed.Invoke(); 
    }

    void onEntityDead()
    {
        Destroy(gameObject);
    }
    // Update is called o  nce per frame
    void Update()
    {
        
    }
}
