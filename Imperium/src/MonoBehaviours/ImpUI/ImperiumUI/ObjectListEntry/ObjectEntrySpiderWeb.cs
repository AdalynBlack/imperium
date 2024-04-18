#region

using Imperium.Core;
using Imperium.Netcode;
using Unity.Netcode;

#endregion

namespace Imperium.MonoBehaviours.ImpUI.ImperiumUI.ObjectListEntry;

internal class ObjectEntrySpiderWeb : ObjectEntry
{
    protected override bool CanRespawn() => true;
    protected override bool CanDrop() => false;
    protected override bool CanTeleportHere() => true;

    protected override void Respawn()
    {
        Destroy();
        ObjectManager.SpawnMapHazard("SpiderWeb", containerObject.transform.position);
    }

    public override void Destroy()
    {
        base.Destroy();
        ImpNetSpawning.Instance.DespawnMapHazardServerRpc(
            containerObject.GetComponent<NetworkObject>().NetworkObjectId
        );
    }
    
    protected override void TeleportHere()
    {
        var origin = Imperium.Freecam.IsFreecamEnabled.Value ? Imperium.Freecam.transform : null;
        Imperium.ImpPositionIndicator.Activate(position => GetContainerObject().transform.position = position, origin);
    }

    protected override string GetObjectName() => $"Spider Web <i>{component.GetInstanceID()}</i>";
}