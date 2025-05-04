using UnityEngine;

public class Town : Building, IResourceHarvester
{
    public override void Init(Pawn ownerPawn, int tileID)
    {
        base.Init(ownerPawn, tileID);
        m_BuildingType = BuildingType.Town;
    }

    public override void EndTurn()
    {
    }

    public override void StartTurn()
    {
    }

    public void ObtainResource(ResourceType resourceType, int amount)
    {
        m_OwnerPawn.ObtainResource(resourceType, amount);
    }
}
