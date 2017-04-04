using Improbable.Math;
using Improbable.Worker;
using Improbable.General;
using Improbable.Unity.Core.Acls;
using Terrain = Improbable.Terrain.Terrain;
using TerrainData = Improbable.Terrain.TerrainData;
using UnityEngine;
using Improbable.Crate;
using Improbable;
using Improbable.Collections;

public class CrateEntityTemplate : MonoBehaviour
{
    public static SnapshotEntity GenerateCrateEntityTemplate(Coordinates coord)
    {
        var crate = new SnapshotEntity { Prefab = "Crate" };
        crate.Add(new Crate.Data(new CrateData()));
        crate.Add(new WorldTransform.Data(new WorldTransformData(coord, 0)));
        crate.Add(new Controllable.Data());

        var permissions = Acl.Build()
            .SetReadAccess(CommonPredicates.PhysicsOrVisual)
            .SetWriteAccess<EntityAcl>(CommonPredicates.PhysicsOnly)
            .SetWriteAccess<Crate>(CommonPredicates.PhysicsOnly)
            .SetWriteAccess<WorldTransform>(CommonPredicates.PhysicsOnly)
            .SetWriteAccess<Controllable>(CommonPredicates.PhysicsOnly);

        crate.SetAcl(permissions);

        return crate;
    }
}
