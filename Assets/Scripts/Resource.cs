using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    None,
    Science,
    Gold,
    Food,
    Production,
    Sword
}

public interface IResourceHarvester
{
    public void ObtainResource(ResourceType resourceType, int amount);
}

public class Resource : GridObject
{
    private HexTile m_Tile;
    private ResourceType m_ResourceType;
    
    private int m_CurrentResourceCooldown;
    [SerializeField] private int m_InitialResourceCooldown = 8;

    public HexTile Tile { get => m_Tile; set => m_Tile = value; }

    public void Init(HexTile tile, ResourceType resourceType)
    {
        m_Tile = tile;
        m_ResourceType = resourceType;
        
        transform.name = "Resource";
        var visual = Instantiate(HexGrid.s_Instance.GridObjectPrefabSettings.GetResourcePrefab(resourceType));
        visual.transform.name = "Visual";
        visual.transform.SetParent(transform);
        visual.transform.localPosition = new Vector3(0.0f, 0.7f, 0.0f);
    }

    void Start()
    {
        base.OnStart();
        m_CurrentResourceCooldown = 0;
    }

    public override void StartTurn()
    {
        var coords = HexGrid.s_Instance.NeighborTileCoords(m_Tile.OffsetCoordinate);
        foreach (var coord in coords)
        {
            HexTile tile = HexGrid.s_Instance.GetTile(coord);
        
            if(tile.Pawn && tile.Pawn is IResourceHarvester)
            {
                var resource = HarvestResource(1);
                (tile.Pawn as IResourceHarvester).ObtainResource(resource.type, resource.givenAmount);
            }
            else if(tile.Building && tile.Building is IResourceHarvester)
            {
                var resource = HarvestResource(1);
                (tile.Building as IResourceHarvester).ObtainResource(resource.type, resource.givenAmount);
            }
                
        }
    }
    
    public override void EndTurn()
    {
        if(m_CurrentResourceCooldown > 0)
        {
            m_CurrentResourceCooldown--;   
        }
    }
    
    public (ResourceType type, int givenAmount) HarvestResource(int amount)
    {
        if(m_ResourceType == ResourceType.None) 
            return(m_ResourceType, 0);
    
        if(m_CurrentResourceCooldown > 0)
            return (m_ResourceType, 0);
        else
        {
            m_CurrentResourceCooldown = m_InitialResourceCooldown;
            return (m_ResourceType, amount);
        }
    }
    
}
