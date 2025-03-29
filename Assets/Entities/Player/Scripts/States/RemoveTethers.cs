using UnityEngine;
using UnityEngine.Events;

public class RemoveTethers : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject[] tethers = GameObject.FindGameObjectsWithTag("TetherPoint");
    foreach (GameObject obj in tethers)
        {
            TetherPoint tether = obj.GetComponent<TetherPoint>();
            if (tether.playerController.gameObject == animator.gameObject)
            {
                tether.RemoveTether();
            }
        }
    }

}
