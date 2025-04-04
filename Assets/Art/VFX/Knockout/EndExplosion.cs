using System.Collections;
using UnityEngine;

public class EndExplosion : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        KOEffectManager manager = animator.gameObject.GetComponent<KOEffectManager>();
        manager.DestructionDelay(animator);
    }

 
}
