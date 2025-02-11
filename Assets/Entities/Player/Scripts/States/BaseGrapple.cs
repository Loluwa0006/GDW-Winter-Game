using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent
    (typeof(LineRenderer))]
[RequireComponent (typeof(DistanceJoint2D))]
public class BaseGrapple : Base_State
    
{
    DistanceJoint2D _distanceJoint;
    LineRenderer _grappleLine;

    Vector3 _grapplePoint;

 //   [SerializeField] float jointdistance = 4.5f;
    [SerializeField] float jointDamping = 7.0f;

    //  [SerializeField] float jointMassScale = 4.5f;

    // [SerializeField] float jointMaxDistance = 0.8f;
    //  [SerializeField] float jointMinDistance = 0.25f;

    Rigidbody2D _rb;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        if (!stateInitalized)
        {
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
            _ssControls = playerController._ssControls;

            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
      

        }
        _grappleLine = animator.gameObject.AddComponent<LineRenderer>();
        _distanceJoint = animator.gameObject.AddComponent<DistanceJoint2D>();

        _grapplePoint = new Vector3(animator.GetFloat("GrapplePointX"), animator.GetFloat("GrapplePointY"), 1);
        _distanceJoint.autoConfigureConnectedAnchor = false;
        _distanceJoint.connectedAnchor = _grapplePoint;
       _distanceJoint.enableCollision = true;

      //  _distanceJoint. = jointDamping;
      //  

        //_distanceJoint.sp        
    }

    void DrawRope()
    {
        if (_grappleLine != null)
        {
            _grappleLine.SetPosition(0, playerController.transform.position);
            _grappleLine.SetPosition(1, _grapplePoint);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        animator.SetBool("GrapplePressed", Input.GetMouseButton(0));
        DrawRope();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Destroy(_grappleLine);
        Destroy(_distanceJoint);
    }

}
