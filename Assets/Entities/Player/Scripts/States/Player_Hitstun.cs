using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class Player_Hitstun : Base_State
{
    Rigidbody2D _rb;
    [SerializeField] float hitstunGravity = 0.35f;
    float originalGravity = 0.0f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInitalized)
        {
            animator.updateMode = AnimatorUpdateMode.Fixed;
            _rb = animator.GetComponent<Rigidbody2D>();
            originalGravity = _rb.gravityScale;

            stateInitalized = true;

            //Shrink ground collider size to make sure player is standing on top of something
            //Without this the player would stick to walls by having the furthest parts of the model touch said wall

        }
        _rb.gravityScale = hitstunGravity;

        Debug.Log("Stunned for " + animator.GetInteger("HitstunAmount").ToString());


    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetInteger("HitstunAmount", animator.GetInteger("HitstunAmount") - 1);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        _rb.gravityScale = originalGravity;
    }
}
