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
 
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInitalized)
        {
            animator.updateMode = AnimatorUpdateMode.Fixed;
            _rb = animator.GetComponent<Rigidbody2D>();

            stateInitalized = true;

            //Shrink ground collider size to make sure player is standing on top of something
            //Without this the player would stick to walls by having the furthest parts of the model touch said wall

        }
        _rb.sharedMaterial.bounciness = hitstunBounce;
        _rb.gravityScale = hitstunGravity;

        _rb.linearDamping = speedDecay;

        Debug.Log("Stunned for " + animator.GetInteger("HitstunAmount").ToString());

    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("HitstunAmount", animator.GetInteger("HitstunAmount") - 1);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _rb.gravityScale = 1.0f;
        _rb.linearDamping = 0.0f;
        _rb.sharedMaterial.bounciness = 0.0f;
    }
}
