using Newtonsoft.Json.Bson;
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


    float groundCheckerLength = 0.85f;

    BoxCollider2D _boxCollider;




    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


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
            groundMask = LayerMask.GetMask("Ground", " Wall");
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
            _boxCollider = playerController.GetHurtbox(); ;

            stateInitalized = true;

            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
            InitInputActions(animator);

        }

        _grapplePoint = new Vector3(animator.GetFloat("GrapplePointX"), animator.GetFloat("GrapplePointY"), 1);
        DrawRope();
        _distanceJoint.connectedAnchor = _grapplePoint;
        _grappleLine.enabled = true;
        _distanceJoint.enabled = true;
        _rb.gravityScale -= GRAPPLE_GRAVITY_REDUCTION;




    }

    protected void DrawRope()
    {

        if (_grappleLine != null)
        {
            _grappleLine.SetPosition(0, playerController.transform.position);
            _grappleLine.SetPosition(1, _grapplePoint);
        }

    }

    RaycastHit2D nearGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerController.transform.position, new Vector2(0, -1), groundCheckerLength, groundMask);
        Color rayColor = Color.red;
        if (hit)
        {
            rayColor = Color.green;
        }
        Debug.DrawLine(playerController.transform.position, new Vector2(playerController.transform.position.x, playerController.transform.position.y - 1), rayColor);
        return hit;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

   //     RaycastHit2D hit = nearGround();
        ////if (hit)
        ////{
        ////    Debug.Log("ground anti magnetism go!!!");
        ////    _distanceJoint.distance -= 0.05f;
        ////    Vector2 grappleDistance = (playerController.transform.position - hit.transform.position);
        ////    _distanceJoint.distance = grappleDistance.magnitude - (_boxCollider.size.y + 0.6f);
        ////    This is to force the player off the ground so they can keep swinging in case they go from land to air
        ////}



            DrawRope();
    }

    protected override void InitInputActions(Animator animator)
    {
        playerInput.actions["Grapple"].performed += ctx => animator.SetBool("GrapplePressed", true);
        playerInput.actions["Grapple"].canceled += ctx => animator.SetBool("GrapplePressed", false);
    }


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _distanceJoint.enabled = false;
        _grappleLine.enabled = false;

        _rb.gravityScale += GRAPPLE_GRAVITY_REDUCTION;
    }

}