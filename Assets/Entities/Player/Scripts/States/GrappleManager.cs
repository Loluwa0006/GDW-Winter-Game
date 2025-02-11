using UnityEngine;
using UnityEngine.InputSystem;

public class GrappleManager : Base_State
{
    [SerializeField] LayerMask _grapplableLayers;

  [SerializeField]  float _maxDistance = 35.0f;



    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!stateInitalized)
        {
            _ssControls = new ShadowStrideControls();
            groundMask = LayerMask.GetMask("Ground");
            playerInput = animator.GetComponentInParent<PlayerInput>();
            playerController = animator.GetComponent<PlayerController>();
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

        Debug.Log("Mouse pressed = " + Input.GetMouseButton(0));

        
        animator.SetBool("GrapplePressed", Input.GetMouseButton(0));
    }

    void InitInputActions(Animator animator)
    {
        //_ssControls.ShadowStridePlayer.Grapple.started += ctx => _grapplePressed = true;
        //_ssControls.ShadowStridePlayer.Grapple.canceled += ctx => _grapplePressed = false;

        //_ssControls.ShadowStridePlayer.Tether.started += ctx => animator.SetBool("TetherPressed", true);
        //_ssControls.ShadowStridePlayer.Tether.canceled += ctx => animator.SetBool("TetherPressed", false);

    }

}
