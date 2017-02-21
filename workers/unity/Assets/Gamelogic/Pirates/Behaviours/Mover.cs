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
    [EngineType(EnginePlatform.Client)]
    public class Mover : MonoBehaviour
    {
        [Require]
        private WorldTransform.Writer WorldTransformWriter;
        public Transform trackTo;

        void Update()
        {
            if (WorldTransformWriter != null && trackTo != null)
            {
                WorldTransformWriter.Send(new WorldTransform.Update()
                    .SetPosition(trackTo.position.ToCoordinates())
                    .SetRotation((uint)trackTo.rotation.eulerAngles.y));
            }
        }
    }
}