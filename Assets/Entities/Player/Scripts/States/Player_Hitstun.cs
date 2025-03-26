using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class Player_Hitstun : Base_State
{
    Rigidbody2D _rb;
    [SerializeField] float hitstunGravity = 0.35f;
    [SerializeField] float speedDecay = 0.02f;

    //time until decay is relative to hitstun amount;

    [SerializeField] float hitstunBounce = 0.85f;

    bool decreaseHitstun = false;

    Transform spriteObject;

    const int  SHAKESCALE = 5;
 
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInitalized)
        {
            animator.updateMode = AnimatorUpdateMode.Fixed;
            _rb = animator.GetComponent<Rigidbody2D>();
            playerController = _rb.gameObject.GetComponent<PlayerController>();
            stateInitalized = true;
            spriteObject = playerController.playerSprite.transform;

            //Shrink ground collider size to make sure player is standing on top of something
            //Without this the player would stick to walls by having the furthest parts of the model touch said wall

        }
        decreaseHitstun = false;

        _rb.sharedMaterial.bounciness = hitstunBounce;
        _rb.gravityScale = hitstunGravity;

        _rb.linearDamping = speedDecay;

        Debug.Log("Stunned for " + animator.GetInteger("HitstunAmount").ToString());

        _rb.linearVelocity = Vector2.zero;
        

    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (decreaseHitstun)
        {
            animator.SetInteger("HitstunAmount", animator.GetInteger("HitstunAmount") - 1);
        }
        else
        {
            ShakePlayerSprite(animator);
        }
    }


    void ShakePlayerSprite(Animator animator)
    {
        float shakeAmount = animator.GetFloat("HitshakeAmount");
        float xShake = Random.Range(-shakeAmount, shakeAmount); 
        float yShake = Random.Range(shakeAmount, shakeAmount);
        spriteObject.transform.localPosition = new Vector2(xShake, yShake);

        shakeAmount = Mathf.Lerp(shakeAmount, 0, Time.deltaTime * SHAKESCALE);


        if (shakeAmount <= 0.01f)
        {

            Vector2 push = new Vector2(animator.GetFloat("KnockbackX"), animator.GetFloat("KnockbackY"));
            _rb.AddForce(push);
            shakeAmount = 0;
            spriteObject.transform.localPosition = Vector2.zero;
            decreaseHitstun = true;
        }
        animator.SetFloat("HitshakeAmount", shakeAmount);

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _rb.gravityScale = 1.0f;
        _rb.linearDamping = 0.0f;
        _rb.sharedMaterial.bounciness = 0.0f;
        animator.SetFloat("HitshakeAmount", 0.0f);
    }
}
