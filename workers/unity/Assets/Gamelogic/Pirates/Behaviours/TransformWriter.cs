using UnityEngine;
using Improbable.General;
using Improbable.Math;
using Improbable.Unity.Visualizer;
using Improbable.Unity;
using Improbable;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Unity.Core;
using Improbable.Worker;
using Improbable.Collections;
using Improbable.Unity.Core.Acls;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Crate;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    [EngineType(EnginePlatform.FSim)]
    public class TransformWriter : MonoBehaviour
    {
        [Require]
        private WorldTransform.Writer WorldTransformWriter;

        void OnEnable()
        {
            WorldTransformWriter.CommandReceiver.OnTakeControl += CommandReceiver_OnTakeControl;
        }

        void OnDisable()
        {
            WorldTransformWriter.CommandReceiver.OnTakeControl -= CommandReceiver_OnTakeControl;
        }

        private void CommandReceiver_OnTakeControl(ResponseHandle<WorldTransform.Commands.TakeControl, ControlRequest, Nothing> obj)
        {
            if(obj.Request.action == "release")
            {
                ReleaseControl(WorldTransformWriter.EntityId);
            } else
            {
                GainControl(WorldTransformWriter.EntityId, obj.CallerInfo.CallerWorkerId);
            }
            obj.Respond(new Nothing());

        }

        public void GainControl(EntityId entityId, string workerId)
        {
            Debug.Log(workerId + " is attempting to gain control of: " + entityId);

            var query = Query.HasEntityId(entityId).ReturnAllComponents();
            SpatialOS.Commands.SendQuery(WorldTransformWriter, query, result => {
                if (result.StatusCode != StatusCode.Success)
                {
                    Debug.Log("query failed with error: " + result.ErrorMessage);
                    return;
                }
                if (!result.Response.HasValue)
                {
                    return;
                }
                Debug.Log("Setting entity " + entityId + " to be controlled by worker " + workerId);

                Entity playerEntity = result.Response.Value.Entities.First.Value.Value;
                Debug.Log("Has authority over EntityAcl " + playerEntity.HasAuthority<EntityAcl>());
                Debug.Log("Has authority over WorldTransform " + playerEntity.HasAuthority<WorldTransform>());
                Debug.Log("Has authority over Crate " + playerEntity.HasAuthority<Crate>());

                Acl acl = Acl.GenerateClientAuthoritativeAcl(playerEntity, workerId);

                playerEntity.SetAcl(acl);
                Debug.Log("Has authority over after SetACL " + playerEntity.HasAuthority<EntityAcl>());
                Debug.Log("Has authority over EntityAcl after SetACL" + playerEntity.HasAuthority<EntityAcl>());
                Debug.Log("Has authority over WorldTransform after SetACL" + playerEntity.HasAuthority<WorldTransform>());
                Debug.Log("Has authority over Crate after SetACL" + playerEntity.HasAuthority<Crate>());
            });
        }

        public void ReleaseControl(EntityId entityId)
        {
            var query = Query.HasEntityId(entityId).ReturnAllComponents();

            SpatialOS.Commands.SendQuery(WorldTransformWriter, query, result => {
                if (result.StatusCode != StatusCode.Success)
                {
                    Debug.Log("query failed with error: " + result.ErrorMessage);
                    return;
                }
                if (!result.Response.HasValue)
                {
                    return;
                }

                Map<EntityId, Entity> resultMap = result.Response.Value.Entities;
                Entity playerEntity = resultMap.First.Value.Value;
                Acl acl = Acl.Build().SetWriteAccess<WorldTransform>(CommonPredicates.PhysicsOnly);
                playerEntity.SetAcl(acl);
            });
        }
    }
}