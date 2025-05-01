using UnityEngine;

public enum TileType
{
    None,
    Ocean,
    Plain,
    Hill,
    Mountain
}

public class HexTile : GridObject
{
    [SerializeField] private TileType m_TileType = TileType.Plain;

    private Pawn m_Pawn;
    private Building m_Building;
    private Resource m_Resource;

    public HexRenderer Renderer;
    [SerializeField] private GameObject m_Fog;
    private bool m_IsFogged;
    
    private int m_TileId;
    private Vector2Int m_OffsetCoordinate;
    private Vector3Int m_CubeCoordinates;

    public int TileId { get => m_TileId; set => m_TileId = value; }
    public Vector2Int OffsetCoordinate { get => m_OffsetCoordinate; }
    public Vector3Int CubeCoordinates { get => m_CubeCoordinates; }
    
    public Resource Resource { get => m_Resource; }
    public Pawn Pawn { get => m_Pawn; set => m_Pawn = value; }
    public Building Building { get => m_Building; }
    public bool IsFogged { get => m_IsFogged; }
    public TileType TileType { get => m_TileType; set => m_TileType = value; }

    public override void OnStart()
    {
        base.OnStart();
        
        m_Fog.GetComponent<HexRenderer>().DrawMesh();
    }
    
    public bool IsWalkable()
    {
        switch (m_TileType)
        {
            case TileType.Ocean:
                return false;
            default:
                return true;
        }
    }
    
    public override void StartTurn()
    {
    }

    public override void EndTurn()
    {   
        if(m_Pawn)
        {
            if(m_IsFogged)
            {
                m_Pawn.SetVisible(false);
            }
            else
            {
                m_Pawn.SetVisible(true);
            }
        }
    }
    
    public void SetResource(ResourceType resourceType)
    {
        GameObject resource = new GameObject();
        resource.transform.SetParent(transform);
        resource.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        m_Resource = resource.AddComponent<Resource>();
        m_Resource.Init(this, resourceType);
    }
    
    public bool CanPossesBuilding(Building building)
    {
        if(m_Building || m_TileType == TileType.Mountain)
        {
            return false;   
        }
        return true;
    }
    
    public bool PossesBuilding(Building building)
    {
        if(!CanPossesBuilding(building)) return false;
        
        m_Building = building;
        m_Building.transform.position = transform.position;
        return true;
    }
    
    public void SetFog(bool state)
    {
        if(state)
        {
            Renderer.gameObject.layer = LayerMask.NameToLayer("Hidden");
        }
        else
        {
            Renderer.gameObject.layer = LayerMask.NameToLayer("Default");
        }
        m_Fog.SetActive(state);
        m_IsFogged = state;
    }
    
    public void SetFogDebug(bool state)
    {
        if(state)
        {
            Renderer.gameObject.layer = LayerMask.NameToLayer("Hidden");
        }
        else
        {
            Renderer.gameObject.layer = LayerMask.NameToLayer("Default");
        }
        m_Fog.SetActive(state);
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
