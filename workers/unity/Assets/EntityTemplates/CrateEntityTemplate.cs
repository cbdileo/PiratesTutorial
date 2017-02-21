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
    public static SnapshotEntity GenerateCrateEntityTemplate()
    {
        var crate = new SnapshotEntity { Prefab = "Crate" };
        crate.Add(new Crate.Data(new CrateData()));
        crate.Add(new WorldTransform.Data(new WorldTransformData(new Coordinates(1, 6, 9), 0)));

        // Define the worker claims
        var fsimClaim = new WorkerClaim(new List<WorkerClaimAtom> { new WorkerClaimAtom("physics") });
        var clientClaim = new WorkerClaim(new List<WorkerClaimAtom> { new WorkerClaimAtom("visual") });

        // Define the two worker predicates
        var fsimOnlyPredicate = new WorkerPredicate(new List<WorkerClaim> { fsimClaim });
        var fsimOrClientPredicate = new WorkerPredicate(new List<WorkerClaim> { fsimClaim, clientClaim });

        var readPermissions = fsimOrClientPredicate;
        var writePermissions = new Map<uint, WorkerPredicate>();

        writePermissions.Add(EntityAcl.ComponentId, fsimOnlyPredicate);
        writePermissions.Add(Crate.ComponentId, fsimOnlyPredicate);
        writePermissions.Add(WorldTransform.ComponentId, fsimOnlyPredicate);
        var componentAcl = new ComponentAcl(writePermissions);

        crate.Add(new EntityAcl.Data(new EntityAclData(readPermissions, componentAcl)));
        return crate;
    }
}
