using Improbable.Math;
using Improbable.Worker;
using Improbable.General;
using Improbable.Unity.Core.Acls;
using Terrain = Improbable.Terrain.Terrain;
using TerrainData = Improbable.Terrain.TerrainData;
using UnityEngine;
using Improbable.Crate;

public class CrateEntityTemplate : MonoBehaviour
{
    public static SnapshotEntity GenerateCrateEntityTemplate()
    {
        var crate = new SnapshotEntity {Prefab = "Crate"};
        crate.Add(new Crate.Data(new CrateData()));
        crate.Add(new WorldTransform.Data(new WorldTransformData(new Coordinates(1, 6, 9), 0)));

        var acl = Acl.GenerateServerAuthoritativeAcl(crate);
        crate.SetAcl(acl);

        return crate;
    }
}
