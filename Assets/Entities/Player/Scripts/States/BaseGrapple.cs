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

    const float GRAPPLE_GRAVITY = 0.8f;

     

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
            groundMask = LayerMask.GetMask("Ground"," Wall");
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

            _rb = animator.gameObject.GetComponent<Rigidbody2D>();
            InitInputActions(animator);
      

        }
      
        _grapplePoint = new Vector3(animator.GetFloat("GrapplePointX"), animator.GetFloat("GrapplePointY"), 1);
        DrawRope();
        // Physics2D.overl
        _distanceJoint.connectedAnchor = _grapplePoint;
        _grappleLine.enabled = true;
        _distanceJoint.enabled = true;
        _rb.gravityScale = GRAPPLE_GRAVITY;

      //  _distanceJoint. = jointDamping;
      //  

        //_distanceJoint.sp        
    }

    protected void DrawRope()
    {

        _distanceJoint.enabled = !IsGrounded();
        if (_grappleLine != null)
        {
            _grappleLine.SetPosition(0, playerController.transform.position);
            _grappleLine.SetPosition(1, _grapplePoint);
        }

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);



        DrawRope();
    }
    void InitInputActions(Animator animator)
    {
   

        playerInput.actions["Grapple"].performed += ctx => animator.SetBool("GrapplePressed", true);

        playerInput.actions["Grapple"].canceled += ctx => animator.SetBool("GrapplePressed", false);



    }


    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _distanceJoint.enabled = false;
        _grappleLine.enabled = false;

        _rb.gravityScale = 1.0f;
    }

}
