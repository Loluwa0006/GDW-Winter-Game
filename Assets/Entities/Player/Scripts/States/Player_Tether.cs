using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;


[System.Serializable]
public class Player_Tether : Base_State

{
    [SerializeField] TetherPoint tetherPointPrefab;
    
    List<TetherPoint> tetherPoints = new List<TetherPoint>();
  
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      // base.OnStateEnter(animator, stateInfo, layerIndex);
       if (!stateInitalized)
        {
            playerController = animator.gameObject.GetComponent<PlayerController>();

        }
       foreach (TetherPoint tether in tetherPoints)
        {
            if (tether == null)
            {
                tetherPoints.Remove(tether);
            }
            //another state clears all tethers, so we need to make sure that they still exist when we enter the state
        }
        TetherPoint newTether = Instantiate(tetherPointPrefab);

        Vector2 tetherDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tetherDirection = tetherDirection - new Vector2(animator.transform.position.x, animator.gameObject.transform.position.y);

        newTether.transform.position = animator.gameObject.transform.position;



        tetherPoints.Add(newTether);
        newTether.FireTether(tetherDirection);

       
        newTether.name = "Tether " + tetherPoints.IndexOf(newTether).ToString();

        if (tetherPoints.Count > 2)
        {
            if (tetherPoints[0] != null)
            {
                Destroy(tetherPoints[0].gameObject);
            }
            tetherPoints[1].breakLink();
            tetherPoints.RemoveAt(0);
        }
        //no more than 2 tethers, otherwise we destroy the oldest one

        if (tetherPoints.Count > 1)
        {

                tetherPoints[1].linkToTether(tetherPoints[0]);
            
        }


        Debug.Log(tetherPoints.Count); 
     //   Debug.Log("Trying to connect " + newTether.name + " to tether " + tetherPoints[0].name);

       // Debug.Log("Tether point count is now " + tetherPoints.Count.ToString());
        

    }

   

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("TetherPressed", Input.GetMouseButton(1));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      
    }

}
