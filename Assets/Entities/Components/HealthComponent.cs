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
    public UnityEvent<PlayerController, int> onEntityDead;
    //player that died, lives remaining
    const int minHitstun = 4;
    const float hitstunScale = 0.015f;
    [SerializeField] int _remainingLives = 3;

    public int playerIndex = 0;

    PlayerController player;



    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        if (GameManager.instance != null)
        {
            _remainingLives = (int)GameManager.instance.GetMatchSetting("StockCount");
        }
        else
        {
            Debug.Log("Couldn't find game manager");
        }
        player = GetComponent<PlayerController>();
       
    }
    public virtual void Damage(Vector2 knockback, float damageTaken = 1.0f)
    {
        int stunTime = CalculateStun(knockback);
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

    public int CalculateStun(Vector2 knockback)
    {
        return Mathf.RoundToInt(minHitstun * (knockback.magnitude) * hitstunScale);
        //might be used later for classic attacks
    }


    public virtual void Heal(int heal_amount = 1)
    {
        health -= heal_amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        onEntityHealed.Invoke(0.0f, heal_amount);
    }

    public float GetHealth()
    {
        return health;
    }

    public int GetRemainingLives()
    {
        return _remainingLives;
    }

    public void SetRemainingLives(int lives)
    {
        _remainingLives = lives;
    }

    public void RemoveLife()
    {
        Debug.Log("GRAH, IM COOKED");
        _remainingLives--;
        onEntityDead.Invoke(player, GetRemainingLives());
        health = 0;
    }
    
    // Update is called o  nce per frame
    void Update()
    {

    }
}