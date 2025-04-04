using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[System.Serializable]
public class Player_Hitstun : Base_State
{
    Rigidbody2D _rb;
    public GameObject longHitsparksPrefab;
    public GameObject centerHitsparksPrefab;
    public GameObject knockbackCloudsPrefab;

    [SerializeField] float hitstunGravity = 0.35f;
    [SerializeField] float speedDecay = 0.02f;

    [SerializeField] AudioClip damagedLight;
    [SerializeField] AudioClip damagedMid;
    [SerializeField] AudioClip damagedHeavy;



    int sfxChance = 40;

    //time until decay is relative to hitstun amount;

    [SerializeField] float hitstunBounce = 0.85f;

    bool decreaseHitstun = false;

    Transform spriteObject;

    const int  SHAKESCALE = 5;


    CinemachineGroupFraming groupFraming;
    Camera cam;
    float shakeAmount;
    Vector2 knockback;


    GameObject knockbackClouds;


    float lowKnockback = 15.0f;
    float midKnockback = 60.0f;

    AudioSource gethitAudioSource;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInitalized)
        {
            animator.updateMode = AnimatorUpdateMode.Fixed;
            _rb = animator.GetComponent<Rigidbody2D>();
            playerController = _rb.gameObject.GetComponent<PlayerController>();
            stateInitalized = true;
            spriteObject = playerController.playerSprite.transform;
            groupFraming = GameObject.FindGameObjectWithTag("CinemachineCamera").GetComponent<CinemachineGroupFraming>();
            cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            gethitAudioSource = playerController.playerAudio;
            //Shrink ground collider size to make sure player is standing on top of something
            //Without this the player would stick to walls by having the furthest parts of the model touch said wall

        }
        decreaseHitstun = false;

        _rb.sharedMaterial.bounciness = hitstunBounce;
        _rb.gravityScale = hitstunGravity;

        _rb.linearDamping = speedDecay;


        Vector2 spawnPosition = new Vector2(animator.GetFloat("HitboxCollisionX"), animator.GetFloat("HitboxCollisionY"));
        shakeAmount = animator.GetFloat("HitshakeAmount") * Mathf.Lerp(1.0f, 1.25f, cam.orthographicSize / groupFraming.OrthoSizeRange.y);
        knockback = new Vector2(animator.GetFloat("KnockbackX"), animator.GetFloat("KnockbackY"));

        playerController.transform.localScale = new Vector2(Mathf.Sign(-knockback.x), 1);
       

        LongParticleCreator(animator, spawnPosition) ;
        CenterParticleCreator(animator, spawnPosition) ;

        PlayHitSFX();
    }

    void PlayHitSFX()
    {
        int chance = Random.Range(1, 100);
        if (chance >= sfxChance)
        {

            if (knockback.magnitude <= lowKnockback)
            {
                gethitAudioSource.PlayOneShot(damagedLight);
            }
            else if (knockback.magnitude <= midKnockback)
            {
                gethitAudioSource.PlayOneShot(damagedMid);
            }
            else
            {
                gethitAudioSource.PlayOneShot(damagedHeavy);
            }
        }
    }

    ParticleSystem KnockbackCloudsCreator(Animator animator, Vector2 spawnPosition)
    {
      
        knockbackClouds = Instantiate(knockbackCloudsPrefab);
        ParticleSystem particles = knockbackClouds.GetComponent<ParticleSystem>();
        particles.Stop();
        knockbackClouds.transform.position = playerController.transform.position;
        var newMain = particles.main;
        newMain.duration = (animator.GetInteger("HitstunAmount") * 50) * Time.fixedDeltaTime;
        particles.Play();
        return particles;
    }
    
    void LongParticleCreator(Animator animator, Vector2 spawnPosition)
    {
        GameObject p1 = Instantiate(longHitsparksPrefab);
        ParticleSystem particles = p1.GetComponent<ParticleSystem>();
        p1.transform.position = spawnPosition;
       
    }
    void CenterParticleCreator(Animator animator, Vector2 spawnPosition)
    {
        GameObject p2 = Instantiate(centerHitsparksPrefab);
        ParticleSystem particles = p2.GetComponent<ParticleSystem>();
        p2.transform.position = spawnPosition;
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        int hitstun = animator.GetInteger("HitstunAmount");
        if (decreaseHitstun)
        {
                knockbackClouds.transform.position = playerController.transform.position;
            
            animator.SetInteger("HitstunAmount", hitstun - 1);
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            ShakePlayerSprite(animator);
        }

    }


    void ShakePlayerSprite(Animator animator)
    {

        float xShake = Random.Range(-shakeAmount, shakeAmount); 
        float yShake = Random.Range(-shakeAmount, shakeAmount);
        if (IsGrounded())
        {
            yShake = 0;
        }
        spriteObject.transform.localPosition = new Vector2(xShake, yShake);

        shakeAmount = Mathf.Lerp(shakeAmount, 0, Time.deltaTime * SHAKESCALE);

        Debug.Log("Shook sprite object to location " + spriteObject.transform.localPosition.ToString());
        if (shakeAmount <= 0.01f)
        {
            _rb.AddForce(knockback, ForceMode2D.Impulse);
            shakeAmount = 0;
            spriteObject.transform.localPosition = Vector2.zero;
            decreaseHitstun = true;
            KnockbackCloudsCreator(animator, playerController.transform.position);
        }
        animator.SetFloat("HitshakeAmount", shakeAmount);

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _rb.gravityScale = 1.0f;
        _rb.linearDamping = 0.0f;
        _rb.sharedMaterial.bounciness = 0.0f;
    }
}
