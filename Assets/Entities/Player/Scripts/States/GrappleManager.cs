using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleManager : Base_State
{
    [SerializeField] LayerMask _grapplableLayers;

    [SerializeField] float _maxDistance = 35.0f;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        if (!stateInitalized)
        {
            playerInput = animator.GetComponentInParent<PlayerInput>();
            playerController = animator.GetComponent<PlayerController>();
            groundMask = playerController.groundMask;
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





    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {



      
        Vector3 aimDirection = new Vector2(animator.GetInteger("HorizAxis"), animator.GetInteger("VertAxis"));

        RaycastHit2D hit = Physics2D.Raycast(playerController.transform.position, aimDirection, _maxDistance, _grapplableLayers);
        //Debug.DrawLine(playerController.transform.position,playerController.transform.position + ( aimDirection * _maxDistance), Color.red);

        if (hit)
        {
            animator.SetFloat("GrapplePointX", hit.point.x);
            animator.SetFloat("GrapplePointY", hit.point.y);
        }
        animator.SetBool("InGrappleRange", hit);
        animator.SetBool("IsGrounded", IsGrounded());

        // animator.SetBool("GrapplePressed", playerInput.actions["Grapple"].IsPressed() );
        //Debug.Log("Mouse pressed = " + Input.GetMouseButton(0));
    }



  protected override void InitInputActions(Animator animator)
    {
        playerInput.actions["DestroyTether"].started += ctx => animator.SetBool("DestroyTethers", true);
        playerInput.actions["DestroyTether"].canceled += ctx => animator.SetBool("DestroyTethers", false);
        playerInput.actions["Grapple"].performed += ctx => animator.SetBool("GrapplePressed", true);
        playerInput.actions["Grapple"].canceled += ctx => animator.SetBool("GrapplePressed", false);
        playerInput.actions["Tether"].started += ctx => animator.SetTrigger("TetherPressed");
    }
}