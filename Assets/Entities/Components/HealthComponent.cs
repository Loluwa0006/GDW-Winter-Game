using UnityEngine;
using UnityEngine.Events;
using System;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] int maxHealth = 999;
    [SerializeField] float health = 0;
    //Dont touch in editor, just expose it
    public UnityEvent<float, int> onEntityDamaged;
    public UnityEvent<float, int> onEntityHealed;
    public UnityEvent onEntityMaxDamageReached;



    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
    }

    public virtual void Damage(float damageTaken = 1.0f, int stunTime = 1)
    {

        damageTaken = (float)Math.Round(damageTaken, 2);

        health += damageTaken;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health >= maxHealth)
        {
            onEntityMaxDamageReached.Invoke();
        }


        onEntityDamaged.Invoke(damageTaken, stunTime);

       // Debug.Log("Hit object for damage " + damageTaken.ToString() + " while stunning them for " + stunTime.ToString());

    }

    public float CalculateStun()
    {
        return 0.0f;
        //might be used later for classic attacks
    }


    public virtual void Heal(int heal_amount = 1)
    {
        health -= heal_amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        onEntityHealed.Invoke(0.0f, heal_amount);
    }

    public float getHealth()
    {
        return health;
    }

    
    // Update is called o  nce per frame
    void Update()
    {

    }
}