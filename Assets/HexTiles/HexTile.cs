using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    None,
    Ocean,
    Plain,
    Desert,
    Mountain
}

public class HexTile : GridObject
{
    public TileType TileType = TileType.Plain;
    
    private ResourceType m_Resource = ResourceType.None;
    private MainPawn m_MainPawn;

    public HexRenderer Renderer;
    
    private int m_TileId;
    private Vector2Int m_OffsetCoordinate;
    private Vector3Int m_CubeCoordinates;

    public int TileId { get => m_TileId; set => m_TileId = value; }
    public Vector2Int OffsetCoordinate { get => m_OffsetCoordinate; }
    public Vector3Int CubeCoordinates { get => m_CubeCoordinates; }
    
    public ResourceType Resource { get => m_Resource; set => m_Resource = value; }
    public MainPawn MainPawn { get => m_MainPawn; set => m_MainPawn = value; }

    public override void OnStart()
    {
        base.OnStart();
    }
    
    public void SetCoordinate(Vector2Int offsetCoord)
    {
        m_OffsetCoordinate = offsetCoord;
        m_CubeCoordinates = OddrToCube(m_OffsetCoordinate);
    }
    
    public void SetCoordinate(Vector3Int cubeCoord)
    {
        m_CubeCoordinates = cubeCoord;
        m_OffsetCoordinate = CubeToOddr(m_CubeCoordinates);
    }
    
    public bool IsWalkable()
    {
        switch (TileType)
        {
            case TileType.Ocean:
                return false;
            default:
                return true;
        }
    }
    
    static Vector3Int OddrToCube(Vector2Int offsetCoord)
    {
        int x = offsetCoord.x - (offsetCoord.y - (offsetCoord.y & 1)) / 2;
        int y = offsetCoord.y;
        return new Vector3Int(x, y, -x-y);
    }
    
    static Vector2Int CubeToOddr(Vector3Int cubeCoord)
    {
        int col = cubeCoord.x + (cubeCoord.y - (cubeCoord.y & 1)) / 2;
        int row = cubeCoord.y;
        return new Vector2Int(col ,row);
    }

    public override void StartTurn(HexTile clickedTile)
    {
    }

    public override void EndTurn(HexTile clickedTile)
    {
        if(m_Resource != ResourceType.None)
        {
            List<Vector2Int> neigborTiles = HexGrid.s_Instance.NeighborTileCoords(m_OffsetCoordinate);
            
            foreach (Vector2Int tileCoord in neigborTiles)
            {
                HexTile tile = HexGrid.s_Instance.GetTile(tileCoord);
                if(tile.m_MainPawn != null)
                {
                    Debug.Log("Main pawn is not null in neighbor!");
                    tile.m_MainPawn.EditResource(m_Resource, 1);
                }
            }
        }
    }
}
