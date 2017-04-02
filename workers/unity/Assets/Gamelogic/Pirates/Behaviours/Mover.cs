using UnityEngine;
using Improbable.General;
using Improbable.Unity.Visualizer;
using Improbable.Unity;

namespace Assets.Gamelogic.Pirates.Behaviours
{
    [EngineType(EnginePlatform.Client)]
    public class Mover : MonoBehaviour
    {
        [Require]
        private WorldTransform.Writer WorldTransformWriter;
        public CreateMoreCrates crateCreator;

        public Transform trackTo;
        private Quaternion newRotation;

        void OnEnable()
        {
            WorldTransformWriter.ComponentUpdated += WorldTransformWriter_ComponentUpdated;
        }

        void OnDisable()
        {
            WorldTransformWriter.ComponentUpdated -= WorldTransformWriter_ComponentUpdated;
        }

        private void WorldTransformWriter_ComponentUpdated(WorldTransform.Update obj)
        {
            Debug.Log("WorldTransformWriter_ComponentUpdated!!");
        }

        void Update()
        {
            // If being carried by the client
            if (WorldTransformWriter != null && WorldTransformWriter.HasAuthority && trackTo != null)
            {
                // Ensure crate is kinematic
                if (!GetComponent<Rigidbody>().isKinematic)
                {
                    GetComponent<Rigidbody>().isKinematic = true;
                }

                Vector3 newPosition = (trackTo.position - (transform.forward*3));
                Quaternion newRotation = trackTo.rotation;

                // Update crate position locally 
                transform.position = newPosition;
                transform.rotation = newRotation;

                // Inform other workers and clients of crates position
                WorldTransformWriter.Send(new WorldTransform.Update()
                    .SetPosition(newPosition.ToCoordinates())
                    .SetRotation((uint)newRotation.eulerAngles.y));
            }
            else
            {
                // Ensure crate isn't kinematic is Player no longer carrying
                if (GetComponent<Rigidbody>().isKinematic)
                {
                    GetComponent<Rigidbody>().isKinematic = false;
                }
            }
        }
    }
}