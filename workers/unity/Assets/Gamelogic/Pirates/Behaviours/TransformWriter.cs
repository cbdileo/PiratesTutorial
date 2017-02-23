using UnityEngine;
using Improbable.General;
using Improbable.Unity.Visualizer;
using Improbable.Unity;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    [EngineType(EnginePlatform.FSim)]
    public class TransformWriter : MonoBehaviour
    {
        [Require]
        private WorldTransform.Writer WorldTransformWriter;

        void Update()
        {
            WorldTransformWriter.Send(new WorldTransform.Update()
                .SetPosition(transform.position.ToCoordinates())
                .SetRotation((uint)transform.rotation.y)
            );
        }
    }
}