using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleManager : Base_State
{
    [SerializeField] LayerMask _grapplableLayers;

  [SerializeField]  float _maxDistance = 35.0f;

    Animator animator;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!stateInitalized)
        {
            this.animator = animator;
            groundMask = LayerMask.GetMask("Ground");
            playerInput = animator.GetComponentInParent<PlayerInput>();
            playerController = animator.GetComponent<PlayerController>();
            _ssControls = playerController._ssControls;
            if (useFixedUpdate)
            {
                animator.updateMode = AnimatorUpdateMode.Fixed;
            }
            else
            {
                animator.updateMode = AnimatorUpdateMode.Normal;
            }
            stateInitalized = true;
            InitInputActions(animator);

        }
        //Debug.Log(animator == null);
        //Debug.Log(animator.layerCount.ToString ());
        //Debug.Log(animator.GetLayerIndex("Grapple"));
        //Debug.Log(layerIndex.ToString());
        animator.Play(animator.GetCurrentAnimatorClipInfo(0)[0].clip.name, layerIndex);




    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        Vector2 directiontomouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        directiontomouse = directiontomouse - new Vector2 (playerController.transform.position.x, playerController.transform.position.y);
        

        RaycastHit2D hit = Physics2D.Raycast(playerController.transform.position, directiontomouse.normalized, _maxDistance, _grapplableLayers);
        if (hit)
        {
            animator.SetFloat("GrapplePointX", hit.point.x);
            animator.SetFloat("GrapplePointY", hit.point.y);
            Debug.DrawLine(playerController.transform.position, hit.point, Color.red);
        }
        animator.SetBool("InGrappleRange", hit);
        animator.SetBool("IsGrounded", IsGrounded());

        animator.SetBool("GrapplePressed", Input.GetMouseButton(0));
        //Debug.Log("Mouse pressed = " + Input.GetMouseButton(0));

    }

    private void onGrapplePressed()
    {
        Debug.Log("grappling button pressed");
    }


    void InitInputActions(Animator animator)
    {
        _ssControls.ShadowStridePlayer.DestroyTether.started += ctx => animator.SetBool("DestroyTethers", true);
        _ssControls.ShadowStridePlayer.DestroyTether.canceled += ctx => animator.SetBool("DestroyTethers", false);

        Debug.Log("grapple actions unlocked");

        _ssControls.ShadowStridePlayer.Grapple.started += ctx => animator.SetBool("GrapplePressed",  true);
        _ssControls.ShadowStridePlayer.Grapple.started += ctx => onGrapplePressed();

        _ssControls.ShadowStridePlayer.Grapple.canceled += ctx => animator.SetBool("GrapplePressed", false);

        _ssControls.ShadowStridePlayer.Tether.started += ctx => animator.SetTrigger("TetherPressed");

    }

}
