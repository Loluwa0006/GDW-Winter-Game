using Newtonsoft.Json.Bson;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class BaseGrapple : Base_State

{
    protected DistanceJoint2D _distanceJoint;
    protected LineRenderer _grappleLine;

    protected Vector3 _grapplePoint;

    //   [SerializeField] float jointdistance = 4.5f;
    [SerializeField] protected float jointDamping = 7.0f;

    //  [SerializeField] float jointMassScale = 4.5f;

    // [SerializeField] float jointMaxDistance = 0.8f;
    //  [SerializeField] float jointMinDistance = 0.25f;

    protected Rigidbody2D _rb;

    const float GRAPPLE_GRAVITY_REDUCTION = 0.2f;

    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("GrappleActive", true);

        if (!stateInitalized)
        {
            if (animator.gameObject.GetComponent<LineRenderer>() == null)
            {
                _grappleLine = animator.gameObject.AddComponent<LineRenderer>();
                _distanceJoint = animator.gameObject.AddComponent<DistanceJoint2D>();

            }
            else
            {
                _grappleLine = animator.gameObject.GetComponent<LineRenderer>();
                _distanceJoint = animator.gameObject.GetComponent<DistanceJoint2D>();
            }
            _distanceJoint.autoConfigureConnectedAnchor = false;
            _distanceJoint.enableCollision = true;
            playerInput = animator.gameObject.GetComponent<PlayerInput>();
            playerController = animator.gameObject.GetComponent<PlayerController>();
           
                animator.updateMode = AnimatorUpdateMode.Fixed;

            
            stateInitalized = true;

            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
            InitInputActions(animator);

        }

        SetGrapple(GrappleManager.grappleInfo);

        DrawRope();
        _grappleLine.enabled = true;
        _distanceJoint.enabled = true;
        _rb.gravityScale -= GRAPPLE_GRAVITY_REDUCTION;

        


    }

    protected void DrawRope()
    {

        if (_grappleLine != null && playerController._activeGrapple != null)
        {
            _grappleLine.SetPosition(0, playerController.transform.position);
            _grappleLine.SetPosition(1, playerController._activeGrapple.transform.position);
        }

    }

    void SetGrapple(RaycastHit2D hit)
    {
        if (!hit.collider)
        {
            return;
         
        }
        float rotation = Vector2.Angle(playerController.transform.position, hit.point);
        playerController._activeGrapple = Instantiate(playerController._grapplePrefab, hit.point, Quaternion.Euler(0, 0, rotation));
        playerController._activeGrapple.transform.SetParent(hit.collider.transform, true);
        Debug.Log("Base localScale = " + playerController._grapplePrefab.transform.localScale.ToString());

    }

  
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (playerController._activeGrapple != null)
        {
            animator.SetBool("GrappleActive", true);
            _grapplePoint = playerController._activeGrapple.transform.position;
            _distanceJoint.connectedAnchor = _grapplePoint;
        }
        else
        {
            animator.SetBool("GrappleActive", false);
            animator.SetBool("InGrappleRange", false);
            //just assume for at least 1 frame that the player can't grapple anything else
            //to make sure that they find another valid target
        }



        DrawRope();
        
    }

    protected override void InitInputActions(Animator animator)
    {
        playerInput.actions["Grapple"].canceled += ctx => Destroy(playerController._activeGrapple);
    }


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _distanceJoint.enabled = false;
        _grappleLine.enabled = false;

        _rb.gravityScale += GRAPPLE_GRAVITY_REDUCTION;

    }

}