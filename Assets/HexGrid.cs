using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    
    private Dictionary<int, GameObject> m_TileDict;
    private List<GameObject> m_Tiles;
    private float m_HorizontalSpacing;
    private float m_VerticalSpacing;

    void Awake()
    {
        m_TileDict = new Dictionary<int, GameObject>();
        m_Tiles = new List<GameObject>();
        
        LayoutGrid();
    }

    public void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 50, Color.yellow);
        
        Vector3 mouseWorldPos = new Vector3();
        if(Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            mouseWorldPos = hitInfo.point;
        }
    
        if(Input.GetMouseButtonDown(0))
        {
            Vector2Int tileCoord = WorldToTileCoord(mouseWorldPos);
            Debug.Log("Clicked Tile: " + tileCoord);
            
            var neighborTiles = NeighborTiles(tileCoord);
            
            foreach (var coord in neighborTiles)
            {
                Debug.Log($"Neighbor Tile: {coord}");
                GetTile(coord).transform.GetComponent<MeshRenderer>().material.color = Color.red;
            }
            
            GetTile(tileCoord).transform.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        
        if(Input.GetMouseButtonUp(0))
        {
            foreach (var pair in m_TileDict)
            {
                pair.Value.transform.GetComponent<MeshRenderer>().material.color = Color.white;
            }
        }
    }

    public void LayoutGrid()
    {
        foreach (GameObject tile in m_Tiles)
        {
            Destroy(tile);
        }
        
        m_TileDict.Clear();
        m_Tiles.Clear();
    
        if(m_Tiles.Count > 0)
        {
            foreach (GameObject tile in m_Tiles)
            {
                Destroy(tile);
            }
            m_Tiles.Clear();
        }
    
        int id = 0;
    
        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                GameObject tile = new GameObject($"Tile {x},{y}", typeof(HexRenderer));
                tile.transform.position = TileCoordToPosition(new Vector2Int(x, y));
                
                HexRenderer hexRenderer = tile.GetComponent<HexRenderer>();
                hexRenderer.PointyTop = PointyTop;
                hexRenderer.OuterRadius = OuterRadius;
                hexRenderer.InnerRadius = InnerRadius;
                hexRenderer.Height = Height;
                hexRenderer.SetMaterial(material);
                hexRenderer.DrawMesh();

                tile.transform.SetParent(transform, true);
                
                m_Tiles.Add(tile);
                m_TileDict[id] = tile;
                id++;
            }
        }
    }
    
    private Vector3 TileCoordToPosition(Vector2Int tileCoord)
    {
        float xPos = 0f;
        float yPos = 0f;
    
        float offset;
    
        if(PointyTop)
        {
            bool shouldOffset = tileCoord.y % 2 == 1;
            
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
    
    public List<Vector2Int> NeighborTiles(Vector2Int coord)
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

    private Vector2Int WorldToTileCoord(Vector3 worldPos)
    {
        Vector2Int tileCoord = new Vector2Int();
        Vector2Int roughTileCoord = new Vector2Int();
    
        roughTileCoord.x = (int)(worldPos.x / m_HorizontalSpacing);
        roughTileCoord.y = (int)(worldPos.z / m_VerticalSpacing);
        
        //Debug.Log("Rough Tile Coord: " + roughTileCoord);
        
        List<Vector2Int> possibleCoords = NeighborTiles(roughTileCoord);
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
            GameObject tile = GetTile(possibleCoord);
            float distance = Vector3.Distance(tile.transform.position, worldPos);
            if(distance < leastDistance){
                tileCoord = possibleCoord;
                leastDistance = distance;
            }
        }
        
        return tileCoord;
    }

    private GameObject GetTile(int id)
    {
        if(id < 0 || id > GridSize.x * GridSize.y) return null;
        return m_Tiles[id];
    }
    
    private GameObject GetTile(Vector2Int coord)
    {
        if(coord.x < 0 || coord.x >= GridSize.x || coord.y < 0 || coord.y >= GridSize.y) return null;
        int id = coord.y * GridSize.x + coord.x;
        return GetTile(id);
    }

}
