 using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public struct AStarNode
{
    public int tileId;
    public int originId;
    
    public int GCost;
    public int HCost;
    public int FCost;
}

[Serializable]
struct TilePrefabAttrib
{
    public TileType Type;
    public GameObject Prefab;
}

public class HexGrid : MonoBehaviour, IInputListener
{
    public static HexGrid s_Instance = null;
    
    public MainPawn mainPawn;

    [Header("Grid Settings")]
    public Vector2Int GridSize;
    
    [Header("Tile Prefabs")]
    [SerializeField] private TilePrefabAttrib[] m_TilePrefabAttribs;
    private Dictionary<TileType, GameObject> m_TilePrefabDict;
    
    [Header("Tile Settings")]
    public float Size = 1.0f;
    public bool PointyTop = true;
    public Material material;
    
    public UnityAction<HexTile> OnTileClick;
    
    private List<HexTile> m_Tiles;
    private float m_HorizontalSpacing;
    private float m_VerticalSpacing;

    void Awake()
    {
        if(s_Instance)
        {
            Debug.LogError("Multiple instances of Hex Grid is not allowed.");
            return;   
        }
        else
            s_Instance = this;
    
        m_Tiles = new List<HexTile>();
        
        m_TilePrefabDict = new Dictionary<TileType, GameObject>();
        
        foreach (var attrib in m_TilePrefabAttribs)
        {
            if(!m_TilePrefabDict.ContainsKey(attrib.Type))
            {
                m_TilePrefabDict[attrib.Type] = attrib.Prefab;
            }
        }
        
        LayoutGrid();
    }

    void Start()
    {
        mainPawn = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<MainPawn>();
    
        IInputInvoker[] invokers = GameObject.FindGameObjectWithTag("Input").transform.GetComponents<IInputInvoker>();
        
        foreach (var invoker in invokers)
        {
            invoker.ConnectInput(this);
        }
    }

    public void OnClickInput(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        
        Vector3 InputWorldPos = new Vector3();
        if(Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            InputWorldPos = hitInfo.point;
        }
        
        Vector2Int tileCoord = WorldToTileCoord(InputWorldPos);
        
        HexTile tile = GetTile(tileCoord);
        if(tile == null) return;
        
        OnTileClick?.Invoke(tile);
    }
    
    public Vector2Int ScreenToHexCoord(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        
        Vector3 InputWorldPos = new Vector3();
        if(Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            InputWorldPos = hitInfo.point;
        }
        
        Vector2Int tileCoord = WorldToTileCoord(InputWorldPos);
        
        return tileCoord;
    }
    
    public HexTile MoveToTile(HexTile tileFrom, HexTile tileTo)
    {
        var path = AStarPathfinder.FindPath(tileFrom, tileTo, this);
        if(path.Count <= 1) return null;
        
        return path[1];
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
        if(tile == null) return;

        GUILayout.BeginArea(new Rect(20, 20, 250, 160));
        GUILayout.Label("Tile ID: " + tile.TileId);
        GUILayout.Label("Tile Offset Coordinates: " + tile.OffsetCoordinate);
        GUILayout.Label("Tile Cube Coordinates: " + tile.CubeCoordinates);
        GUILayout.Label("Tile Type: " + tile.TileType.ToString());
        GUILayout.Label("Tile (0,0) to (4,4) distance: " + HexGridDistance(GetTile(new Vector2Int(0, 0)), GetTile(new Vector2Int(4, 4))));
        GUILayout.Label("Main Pawn is at: " + mainPawn.CurrentTile.OffsetCoordinate.ToString());
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
            //Debug.Log("Clicked Tile: " + tileCoord);
            
            var neighborTiles = NeighborTileCoords(tileCoord);
            
            foreach (var coord in neighborTiles)
            {
                //Debug.Log($"Neighbor Tile: {coord}");
                HexTile neighborTile = GetTile(coord);
                if(neighborTile)
                    neighborTile.Renderer.transform.GetComponent<MeshRenderer>().material.color += Color.red;
            }
            
            HexTile tile = GetTile(tileCoord);
            if(tile)
                tile.Renderer.transform.GetComponent<MeshRenderer>().material.color += Color.red;
        }
        
        if(Input.GetMouseButtonUp(0))
        {
            foreach (HexTile tile in m_Tiles)
            {
                tile.Renderer.transform.GetComponent<MeshRenderer>().material.color -= Color.red;
            }
        }
    }

    public void LayoutGrid()
    { 
        if(m_Tiles.Count > 0)
        {
            foreach (HexTile tile in m_Tiles)
            {
                Destroy(tile.gameObject);
            }
            m_Tiles.Clear();
        }
    
        int tileId = 0;
        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                GameObject tileObject;
                
                if(tileId == 2 || tileId == 8)
                {
                    tileObject = Instantiate(m_TilePrefabDict[TileType.Ocean]);
                }
                else
                {
                    tileObject = Instantiate(m_TilePrefabDict[TileType.Plain]);
                }
                
                tileObject.transform.position = TileCoordToPosition(new Vector2Int(x, y));
                
                HexTile tile = tileObject.GetComponent<HexTile>();
                tile.TileId = tileId;
                tile.SetCoordinate(new Vector2Int(x, y));
                
                tile.Renderer.SetPointyTop(PointyTop);
                tile.Renderer.SetSize(Size);
                tile.Renderer.DrawMesh();

                tile.transform.SetParent(transform, true);
                
                if(tileId == 5 || tileId == 9)
                {
                    tile.Resource = ResourceType.Food;
                    tile.transform.GetChild(1).gameObject.SetActive(true);
                }
                
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
            
            m_HorizontalSpacing = Size * MathF.Sqrt(3);
            m_VerticalSpacing = 3f/2f * Size;
            
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
            HexTile tile = GetTile(possibleCoord);
            
            if(tile)
            {
                Vector3 tilePos = tile.transform.position;
                float distance = Vector3.Distance(tilePos, worldPos);
                if(distance < leastDistance){
                    tileCoord = possibleCoord;
                    leastDistance = distance;
                }
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
