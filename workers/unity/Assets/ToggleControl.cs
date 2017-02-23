using Improbable;
using Improbable.Core;
using Improbable.Crate;
using Improbable.Entity.Component;
using Improbable.General;
using Improbable.Unity;
using Improbable.Unity.Core.Acls;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Unity.Core;

[EngineType(EnginePlatform.FSim)]
public class ToggleControl : MonoBehaviour
{
    [Require]
    private Controllable.Writer ControllableWriter;
    [Require]
    private EntityAcl.Writer EntityAclWriter;

    void OnEnable()
    {
        ControllableWriter.CommandReceiver.OnTakeControl += CommandReceiver_OnTakeControl;
    }

    void OnDisable()
    {
        ControllableWriter.CommandReceiver.OnTakeControl -= CommandReceiver_OnTakeControl;
    }

    private void CommandReceiver_OnTakeControl(ResponseHandle<Controllable.Commands.TakeControl, ControlRequest, Nothing> obj)
    {
        Debug.LogWarning("RECEIVED COMMAND " + obj.Request.action);
        if (obj.Request.action == "release")
        {
            ReleaseControl(ControllableWriter.EntityId);
        }
        else
        {
            GainControl(ControllableWriter.EntityId, obj.CallerInfo.CallerWorkerId);
        }
        obj.Respond(new Nothing());
    }

    public void GainControl(EntityId entityId, string workerId)
    {
        Debug.LogWarning(workerId + " is attempting to gain control of: " + entityId);

        EntityAclWriter.Send(
            Acl.Build()
            .SetReadAccess(CommonPredicates.PhysicsOrVisual)
            .SetWriteAccess<EntityAcl>(CommonPredicates.PhysicsOnly)
            .SetWriteAccess<Crate>(CommonPredicates.PhysicsOnly)
            .SetWriteAccess<WorldTransform>(CommonPredicates.SpecificClientOnly(workerId))
            .SetWriteAccess<Controllable>(CommonPredicates.PhysicsOnly)
        );

        // Ensure crate is kinematic
        if (!GetComponent<Rigidbody>().isKinematic)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void ReleaseControl(EntityId entityId)
    {
        Debug.LogWarning("Server regaining control of: " + entityId);

        EntityAclWriter.Send(
            Acl.Build()
            .SetReadAccess(CommonPredicates.PhysicsOrVisual)
            .SetWriteAccess<EntityAcl>(CommonPredicates.PhysicsOnly)
            .SetWriteAccess<Crate>(CommonPredicates.PhysicsOnly)
            .SetWriteAccess<WorldTransform>(CommonPredicates.PhysicsOnly)
            .SetWriteAccess<Controllable>(CommonPredicates.PhysicsOnly)
        );

        // Ensure crate is kinematic
        if (GetComponent<Rigidbody>().isKinematic)
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
