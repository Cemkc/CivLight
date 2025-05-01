using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HexGrid : MonoBehaviour, IInputListener
{
    public static HexGrid s_Instance = null;
    
    public Pawn mainPawn;

    [Header("Grid Settings")]
    public Vector2Int GridSize;
    
    [Header("Grid Prefabs")]
    [SerializeField] private GridObjectPrefabSettings m_GridObjectPrefabSettings;
    
    [Header("Tile Settings")]
    public float Size = 1.0f;
    public bool PointyTop = true;
    public Material material;
    
    public UnityAction<HexTile> OnTileClick;
    
    private List<HexTile> m_Tiles;
    private float m_HorizontalSpacing;
    private float m_VerticalSpacing;

    public GridObjectPrefabSettings GridObjectPrefabSettings { get => m_GridObjectPrefabSettings; }

    void Awake()
    {
        if(s_Instance)
        {
            Debug.LogError("Multiple instances of Hex Grid is not allowed.");
            return;   
        }
        else
        {
            s_Instance = this;   
        }
    
        m_Tiles = new List<HexTile>();
        m_GridObjectPrefabSettings.Init();
        
        LayoutGrid();
    }

    void Start()
    {
        mainPawn = GameObject.FindGameObjectWithTag("MainPawn").transform.GetComponent<Pawn>();
    
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
                
                var type = GetRandomEnumValueExcluding(TileType.Mountain, TileType.None);
                
                tileObject = Instantiate(m_GridObjectPrefabSettings.GetTilePrefab(type));
                
                tileObject.transform.position = TileCoordToPosition(new Vector2Int(x, y));
                
                HexTile tile = tileObject.GetComponent<HexTile>();
                tile.TileId = tileId;
                tile.SetCoordinate(new Vector2Int(x, y));
                
                tile.Renderer.SetPointyTop(PointyTop);
                tile.Renderer.SetSize(Size);
                tile.Renderer.DrawMesh();

                tile.transform.SetParent(transform, true);
                
                if(tile.TileType != TileType.Ocean)
                {
                    if(UnityEngine.Random.Range(0, 100) < 20)
                    {
                        tile.SetResource(GetRandomEnumValueExcluding(ResourceType.None));
                    }
                }
                
                tile.SetFog(false);
                
                m_Tiles.Add(tile);
                tileId++;
            }
        }
    }
    
    public void RemoveFog()
    {
        foreach (HexTile tile in m_Tiles)
        {
            tile.SetFogDebug(false);
        }
    }
    
    public void AddFog()
    {
        foreach (HexTile tile in m_Tiles)
        {
            if(tile.IsFogged)
                tile.SetFogDebug(true);
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

    public void OnAlternateClickInput(Vector2 position)
    {
    }
    
    public static T GetRandomEnumValueExcluding<T>(params T[] excludedValues) where T : System.Enum
    {
        T[] allValues = (T[])System.Enum.GetValues(typeof(T));

        List<T> filteredValues = new List<T>(allValues);
        foreach (var value in excludedValues)
        {
            filteredValues.Remove(value);
        }

        if (filteredValues.Count == 0)
            throw new System.Exception($"No {typeof(T).Name} values available after exclusion!");

        return filteredValues[UnityEngine.Random.Range(0, filteredValues.Count)];
    }
}
