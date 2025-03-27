using UnityEngine;

public enum TileType
{
    None,
    Ocean,
    Plain,
    Desert,
    Mountain
}

public class HexTile : MonoBehaviour
{
    public TileType TileType = TileType.Plain;

    public HexRenderer Renderer;
    
    private int m_TileId;
    private Vector2Int m_OffsetCoordinate;
    private Vector3Int m_CubeCoordinates;

    public Vector2Int OffsetCoordinate { get => m_OffsetCoordinate; }
    public Vector3Int CubeCoordinates { get => m_CubeCoordinates; }
    public int TileId { get => m_TileId; set => m_TileId = value; }

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
    
}
