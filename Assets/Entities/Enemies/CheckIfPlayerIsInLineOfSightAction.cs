using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check if Player is in Line of Sight", story: "[Enemy] Sets [Target] using [VisionLength]", category: "Action", id: "e4048420f5e447b5793360b4c516dfd1")]
public partial class CheckIfPlayerIsInLineOfSightAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector2> Target;
    [SerializeReference] public BlackboardVariable<float> VisionLength;
    [SerializeReference] public BlackboardVariable<GameObject> Self;


    
    protected override Status OnStart()
    {
 
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        Transform enemyTransform = Self.Value.transform;
        Vector2 enemyPosition = enemyTransform.position;
        RaycastHit2D hit = Physics2D.Raycast(enemyPosition, new Vector2(enemyTransform.localScale.x, 0), VisionLength.Value, LayerMask.GetMask("Player"));
        Color rayColor = hit == true ? Color.green : Color.red;
        Debug.DrawLine(enemyPosition, new Vector2(enemyTransform.localScale.x, 0) * VisionLength.Value, rayColor);
        if (hit)
        {
            Target.Value = hit.point;
            return Status.Success;

        }
        return Status.Failure;
    }

    protected override void OnEnd()
    {
    }
}

