using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Look in Direction", story: "Check for Player", category: "Action", id: "159321b3ce55c1034ee948542732ff14")]
public partial class LookInDirectionAction : Action
{

    [SerializeField] Transform enemyTransform;
    [SerializeField] float visionRange = 50.0f;

     
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(enemyTransform.position, new Vector2(enemyTransform.localScale.x, 0), visionRange);
        if (hit)
        {

            return Status.Success;

        }
        return Status.Failure;

    }

    protected override void OnEnd()
    {
    }
}

