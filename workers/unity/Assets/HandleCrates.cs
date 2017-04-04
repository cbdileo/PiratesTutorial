using System.Collections;
using System.Collections.Generic;
using Assets.Gamelogic.Pirates.Behaviours;
using Improbable;
using Improbable.Crate;
using Improbable.General;
using Improbable.Player;
using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;

[EngineType(EnginePlatform.Client)]
public class HandleCrates : MonoBehaviour
{
    [Require] private Handling.Writer HandlingWriter;

    void Update()
    {
        if (HandlingWriter.Data.heldEntity.HasValue)
        {
            EntityId heldEntity = HandlingWriter.Data.heldEntity.Value;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ReleaseControl(heldEntity);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Crate" && !HandlingWriter.Data.heldEntity.HasValue)
        {
            GainControl(other.gameObject);
        }
    }

    public void GainControl(GameObject entityGameObject)
    {
        Debug.Log("GAINING CONTROL");
        EntityId entityId = entityGameObject.EntityId();
        SpatialOS.Commands.SendCommand(HandlingWriter, Controllable.Commands.TakeControl.Descriptor, new ControlRequest("gain"), entityId,
            response =>
            {
                if (response.StatusCode != StatusCode.Failure)
                {
                    Mover mover = entityGameObject.GetComponent<Mover>();
                    mover.trackTo = transform;
                    HandlingWriter.Send(new Handling.Update().SetHeldEntity(entityId));
                    if (entityGameObject.GetComponent<CreateMoreCrates>() == null)
                    {
                        entityGameObject.AddComponent<CreateMoreCrates>();
                    }
                }
            });
    }

    public void ReleaseControl(EntityId entityId)
    {
        Debug.Log("RELEASE CONTROL");
        SpatialOS.Commands.SendCommand(HandlingWriter, Controllable.Commands.TakeControl.Descriptor, new ControlRequest("release"), entityId,
            response =>
            {
                if (response.StatusCode != StatusCode.Failure)
                {
                    HandlingWriter.Send(new Handling.Update().SetHeldEntity(null));
                }
            });
    }
}
