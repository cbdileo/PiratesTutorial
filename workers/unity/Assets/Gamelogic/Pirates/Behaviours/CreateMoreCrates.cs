using Improbable;
using Improbable.Crate;
using Improbable.Math;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[EngineType(EnginePlatform.FSim)]
public class CreateMoreCrates : MonoBehaviour {

    [Require]
    private Crate.Writer CrateWriter;

    void Update() {

    }

    void OnTriggerEnter(Collider other)
    {
        if (CrateWriter == null)
        {
            Debug.Log("Writer Null");
            return;
        }
        if (other != null && other.tag == "Ground")
        {
            Vector3 pos = transform.position;
            var coord = new Coordinates(pos.x, pos.y, pos.z);
            GetComponent<SphereCollider>().enabled = false;

            var createEntityTemplate = CrateEntityTemplate.GenerateCrateEntityTemplate(coord);

            // I could never get this to work because CrateWriter was always null 
            SpatialOS.Commands.CreateEntity(CrateWriter, "Crate", createEntityTemplate, result =>
            //SpatialOS.WorkerCommands.CreateEntity("Crate", createEntityTemplate, result =>
            {
                if (result.StatusCode != StatusCode.Success)
                {
                    Debug.LogError("Failed to create crate: " + result.ErrorMessage);
                    return;
                }
            });

            EntityId id = gameObject.EntityId();
            //SpatialOS.WorkerCommands.DeleteEntity(id, _ => { });
            SpatialOS.Commands.DeleteEntity(CrateWriter, id, _ => { });
        }
    }
}
