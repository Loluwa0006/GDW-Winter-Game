using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Check if Player is in Line of Sight", story: "[EnemyTransform] Sets [Target]", category: "Action", id: "e4048420f5e447b5793360b4c516dfd1")]
public partial class CheckIfPlayerIsInLineOfSightAction : Action
{
    [SerializeReference] public BlackboardVariable<Transform> EnemyTransform;
    [SerializeReference] public BlackboardVariable<Vector2> Target;
    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

