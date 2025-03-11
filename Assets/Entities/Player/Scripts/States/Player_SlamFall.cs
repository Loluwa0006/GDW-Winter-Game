using UnityEngine;
using UnityEngine.Animations;

public class Player_SlamFall : Base_State   
{


    Rigidbody2D _rb;

    public float _slamSpeed = 45.0f;
    [SerializeField] float _slamTurnSpeed = 0.2f;
    [SerializeField] float _maxTurnSpeed = 1.0f;


   
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!stateInitalized)
        {
            _rb = animator.gameObject.GetComponent<Rigidbody2D>();

        }

        base.OnStateEnter(animator, stateInfo, layerIndex);

        

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -_slamSpeed);


        animator.Play(layerIndex);
    }


    
   

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsGrounded", IsGrounded());
        float moveDir = animator.GetInteger("HorizAxis");

        Vector2 newSpeed = _rb.linearVelocity;
        if (Mathf.Abs(newSpeed.x) < _maxTurnSpeed)
        {
            newSpeed.x += _slamTurnSpeed * moveDir;
            newSpeed.x = Mathf.Clamp(moveDir, -_maxTurnSpeed, _maxTurnSpeed);
        }
        
        _rb.linearVelocity = newSpeed;
    }

   /* public override bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerController.transform.position, new Vector2(0, -1), 1.25f, groundMask);
        Color rayColor = Color.red;
        if (hit)
        {
            rayColor = Color.green;
        }
        Debug.DrawLine(playerController.transform.position, playerController.transform.position + new Vector3(0, -1, 0), rayColor);
        return hit;
    } */



}
