using UnityEngine;
using UnityEngine.Events;
using System;
using static HitboxComponent;
using System.Drawing;

public class HealthComponent : MonoBehaviour
{
    int maxHealth = 999;
    float health = 0;
    public UnityEvent<HitboxComponent.HitboxInfo> onPlayerInjured;
    //i'm having 2 functions so i slowly migrate
    public UnityEvent<float, int> onEntityHealed;
    //amount, playerindex
    public UnityEvent onEntityMaxDamageReached;
    public UnityEvent<PlayerController, int> livesChanged;
    //player that died, lives remaining
    public UnityEvent <PlayerController, int> healthInitalized;
    //player that was set up, lives
    const int minHitstun = 4;
    const float hitstunScale = 0.015f;
    const float shakeScale = 0.001f;
    const float BASE_HITSHAKE = 0.25f;
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
        healthInitalized.Invoke(player, _remainingLives);


    }
   

    public virtual void Damage(HitboxComponent.HitboxInfo hitboxInfo)
    {

        health += hitboxInfo.damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health >= maxHealth)
        {
            onEntityMaxDamageReached.Invoke();
        }
        hitboxInfo.stun = CalculateStun(hitboxInfo.push);
        hitboxInfo.shake = CalculateShakeAmount(hitboxInfo.push);
        onPlayerInjured.Invoke(hitboxInfo);
       
    }

    public int CalculateStun(Vector2 knockback)
    {
        return Mathf.RoundToInt(minHitstun * (knockback.magnitude) * hitstunScale);
        //might be used later for classic attacks
    }

    public float CalculateShakeAmount(Vector2 knockback)
    {
        return Mathf.Max(BASE_HITSHAKE,knockback.magnitude * shakeScale - BASE_HITSHAKE);
    }
  


    public virtual void Heal(int heal_amount = 1)
    {
        health -= heal_amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        onEntityHealed.Invoke(0.0f, heal_amount);
    }

    public void SetHealthToMax()
    {
        health = maxHealth;
        onPlayerInjured.Invoke(null);
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
        livesChanged.Invoke(player, lives);
    }

    public void RemoveLife()
    {
        _remainingLives--;
        ResetHealth();
        livesChanged.Invoke(player, GetRemainingLives());
    }

    public void AddLife()
    {
        _remainingLives++;
    }

    public void ResetHealth()
    {
        health = 0;
    }

    // Update is called o  nce per frame
    void Update()
    {

    }
}