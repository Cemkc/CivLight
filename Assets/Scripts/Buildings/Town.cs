using UnityEngine;

public class Town : Building
{
    public override void Init(Pawn ownerPawn, int tileID)
    {
        base.Init(ownerPawn, tileID);
    }

    public override void EndTurn(HexTile clickedTile)
    {
        var neighborTiles = HexGrid.s_Instance.NeighborTileCoords(m_Tile.OffsetCoordinate);
        foreach (Vector2Int coord in neighborTiles)
        {
            HexTile tile = HexGrid.s_Instance.GetTile(coord);
            var resource = tile.HarvestResource(1);
            Debug.Log("I'm harvesting broo");
            m_OwnerPawn.EditResource(resource.type, resource.givenAmount);
        }
    }

    public override void StartTurn(HexTile clickedTile)
    {
    }
}
