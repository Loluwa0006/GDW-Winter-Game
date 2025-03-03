using UnityEngine;

[System.Serializable]
public class GroundGrapple : BaseGrapple
{
    [SerializeField] float pullStrengthAcceleration = 0.1f;
    [SerializeField] float minDistance = 3.0f;

    [SerializeField] float grappleBoost = 8.5f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        _rb.linearVelocity = new Vector2 (_rb.linearVelocity.x + Mathf.Sign(_rb.linearVelocity.x) * grappleBoost , grappleBoost);
     
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    //    base.OnStateUpdate(animator, stateInfo, layerIndex);

        DrawRope();

        _distanceJoint.distance -= pullStrengthAcceleration;
        if (_distanceJoint.distance < minDistance)
        { if (!IsGrounded())
            {
                _distanceJoint.distance = minDistance;
            }
        }

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}