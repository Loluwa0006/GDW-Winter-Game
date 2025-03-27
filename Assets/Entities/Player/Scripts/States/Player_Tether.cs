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

        GameObject[] tetherTags = GameObject.FindGameObjectsWithTag("TetherPoint");
        for (int i = 0; i < tetherTags.Length; i++)
        {
            if (i > 1)
            {
                tetherPoints.Remove(tetherTags[i].GetComponent<TetherPoint>());
                Destroy(tetherTags[i]);
            }
        }

            TetherPoint newTether = Instantiate(tetherPointPrefab);

        Vector2 tetherDirection = new Vector2(animator.GetInteger("HorizAxis"), animator.GetInteger("VertAxis"));
        newTether.transform.position = playerController.transform.position;



        tetherPoints.Add(newTether);
        newTether.FireTether(tetherDirection, playerController);

       
        newTether.name = "Tether " + tetherPoints.IndexOf(newTether).ToString();

        if (tetherPoints.Count > 2)
        {
            if (tetherPoints[0] != null)
            {
                Destroy(tetherPoints[0].gameObject);
            }
            if (tetherPoints[1] != null)
            {
                tetherPoints[1].BreakLink();
            }
                tetherPoints.RemoveAt(0);
        }
        //no more than 2 tethers, otherwise we destroy the oldest one

        if (tetherPoints.Count > 1)
        {

                tetherPoints[1].LinkToTether(tetherPoints[0]);
            
        }


        Debug.Log(tetherPoints.Count); 
     //   Debug.Log("Trying to connect " + newTether.name + " to tether " + tetherPoints[0].name);

       // Debug.Log("Tether point count is now " + tetherPoints.Count.ToString());
        

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }




    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      
    }

}
