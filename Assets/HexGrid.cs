using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct AStarNode
{
    public int tileId;
    public int originId;
    
    public int GCost;
    public int HCost;
    public int FCost;
}

public class HexGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int GridSize;
    
    [Header("Tile Settings")]
    public float OuterRadius = 1f;
    public float InnerRadius = 0f;
    public float Height = 1f;
    public bool PointyTop = true;
    public Material material;
    
    private List<HexTile> m_Tiles;
    private float m_HorizontalSpacing;
    private float m_VerticalSpacing;

    void Awake()
    {
        m_Tiles = new List<HexTile>();
        
        LayoutGrid();
    }
    
    void OnGUI()
    {   
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 50, Color.yellow);
        
        Vector3 mouseWorldPos = new Vector3();
        if(Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            mouseWorldPos = hitInfo.point;
        }
        
        Vector2Int tileCoord = WorldToTileCoord(mouseWorldPos);
        
        HexTile tile = GetTile(tileCoord);

        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Tile ID: " + tile.TileId);
        GUILayout.Label("Tile Offset Coordinates: " + tile.OffsetCoordinate);
        GUILayout.Label("Tile Cube Coordinates: " + tile.CubeCoordinates);
        GUILayout.Label("Tile Type: " + tile.TileType.ToString());
        GUILayout.Label("Tile (0,0) to (4,4) distance: " + HexGridDistance(GetTile(new Vector2Int(0, 0)), GetTile(new Vector2Int(4, 4))));
        GUILayout.EndArea();
    }

    public void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 50, Color.yellow);
        
        if(Input.GetKeyDown(KeyCode.F))
        {
            var path = AStarPathfinder.FindPath(GetTile(0), GetTile(10), this);
            foreach (var tile in path)
            {
                Debug.Log(tile.OffsetCoordinate);
            }
        }
        
        Vector3 mouseWorldPos = new Vector3();
        if(Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            mouseWorldPos = hitInfo.point;
        }
    
        if(Input.GetMouseButtonDown(0))
        {
            Vector2Int tileCoord = WorldToTileCoord(mouseWorldPos);
            Debug.Log("Clicked Tile: " + tileCoord);
            
            var neighborTiles = NeighborTileCoords(tileCoord);
            
            foreach (var coord in neighborTiles)
            {
                Debug.Log($"Neighbor Tile: {coord}");
                GetTile(coord).Renderer.transform.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            
            GetTile(tileCoord).Renderer.transform.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        
        if(Input.GetMouseButtonUp(0))
        {
            foreach (HexTile tile in m_Tiles)
            {
                tile.Renderer.transform.GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }
    }

    public void LayoutGrid()
    { 
        if(m_Tiles.Count > 0)
        {
            foreach (HexTile tile in m_Tiles)
            {
                Destroy(tile);
            }
            m_Tiles.Clear();
        }
    
        int tileId = 0;
        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                HexTile tile = new GameObject($"Tile {x},{y}", typeof(HexTile)).GetComponent<HexTile>();
                tile.Renderer = new GameObject("Mesh", typeof(HexRenderer)).GetComponent<HexRenderer>();
                tile.Renderer.transform.SetParent(tile.transform, false);
                tile.transform.position = TileCoordToPosition(new Vector2Int(x, y));
                
                tile.TileId = tileId;
                tile.SetCoordinate(new Vector2Int(x, y));
                
                if(tile.TileId == 2 || tile.TileId == 8)
                {
                    tile.TileType = TileType.Ocean;
                }
                
                tile.Renderer.PointyTop = PointyTop;
                tile.Renderer.OuterRadius = OuterRadius;
                tile.Renderer.InnerRadius = InnerRadius;
                tile.Renderer.Height = Height;
                tile.Renderer.SetMaterial(material);
                tile.Renderer.DrawMesh();

                tile.transform.SetParent(transform, true);
                
                m_Tiles.Add(tile);
                tileId++;
            }
        }
    }
    
    public Vector3 TileCoordToPosition(Vector2Int tileCoord)
    {
        float xPos = 0f;
        float yPos = 0f;
    
        float offset;
    
        if(PointyTop)
        {
            bool shouldOffset = (tileCoord.y & 1) == 1;
            
            m_HorizontalSpacing = OuterRadius * MathF.Sqrt(3);
            m_VerticalSpacing = 3f/2f * OuterRadius;
            
            offset = m_HorizontalSpacing / 2;
            
            xPos = shouldOffset ? tileCoord.x * m_HorizontalSpacing + offset : tileCoord.x * m_HorizontalSpacing;
                
            yPos = tileCoord.y * m_VerticalSpacing;
        }
        else
        {
            
        }
        
        return new Vector3(xPos, 0.0f, yPos);
    }
    
    public List<Vector2Int> NeighborTileCoords(Vector2Int coord)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        bool evenRow = coord.y % 2 == 0;
        
        if(evenRow)
        {
            neighbors.Add(new Vector2Int(coord.x - 1, coord.y + 1));
            neighbors.Add(new Vector2Int(coord.x, coord.y + 1));
            
            neighbors.Add(new Vector2Int(coord.x - 1, coord.y - 1));
            neighbors.Add(new Vector2Int(coord.x, coord.y - 1));
        }
        else
        {
            neighbors.Add(new Vector2Int(coord.x, coord.y + 1));
            neighbors.Add(new Vector2Int(coord.x + 1, coord.y + 1));
            
            neighbors.Add(new Vector2Int(coord.x, coord.y - 1));
            neighbors.Add(new Vector2Int(coord.x + 1, coord.y - 1));
        }
        
        neighbors.Add(new Vector2Int(coord.x + 1, coord.y));
        neighbors.Add(new Vector2Int(coord.x - 1, coord.y));
        
        neighbors = neighbors.Where(tile => tile.x >= 0 && tile.x < GridSize.x && tile.y >= 0 && tile.y < GridSize.y).ToList();
        
        return neighbors;
    }

    public Vector2Int WorldToTileCoord(Vector3 worldPos)
    {
        Vector2Int tileCoord = new Vector2Int();
        Vector2Int roughTileCoord = new Vector2Int();
    
        roughTileCoord.x = (int)(worldPos.x / m_HorizontalSpacing);
        roughTileCoord.y = (int)(worldPos.z / m_VerticalSpacing);
        
        //Debug.Log("Rough Tile Coord: " + roughTileCoord);
        
        List<Vector2Int> possibleCoords = NeighborTileCoords(roughTileCoord);
        int i = 0;
        foreach (var item in possibleCoords)
        {
            i++;
            //Debug.Log($"Neighbor {i} of rough coord: " + item);
        }

        possibleCoords.Add(roughTileCoord);
        
        float leastDistance = Mathf.Infinity;
        foreach (var possibleCoord in possibleCoords)
        {
            Vector3 tilePos = GetTile(possibleCoord).transform.position;
            float distance = Vector3.Distance(tilePos, worldPos);
            if(distance < leastDistance){
                tileCoord = possibleCoord;
                leastDistance = distance;
            }
        }
        
        return tileCoord;
    }
    
    public int HexGridDistance(HexTile tile1, HexTile tile2)
    {
        Vector3Int tileCoord1 = tile1.CubeCoordinates;
        Vector3Int tileCoord2 = tile2.CubeCoordinates;
        return (int)(MathF.Abs(tileCoord1.x - tileCoord2.x) + MathF.Abs(tileCoord1.y - tileCoord2.y) + MathF.Abs(tileCoord1.z - tileCoord2.z)) / 2 ;
    }

    public HexTile GetTile(int id)
    {
        if(id < 0 || id > GridSize.x * GridSize.y) return null;
        return m_Tiles[id];
    }
    
    public HexTile GetTile(Vector2Int coord)
    {
        if(coord.x < 0 || coord.x >= GridSize.x || coord.y < 0 || coord.y >= GridSize.y) return null;
        int id = coord.y * GridSize.x + coord.x;
        return GetTile(id);
    }
    
 

}
