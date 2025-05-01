using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : MonoBehaviour, IResourceHarvester
{
    private int m_PawnID;
    
    [SerializeField] Vector2Int m_StartPosition;

    public Action<int> OnTurnTaken;
    public Action<ResourceType, int> ResorceChangeEvent;
    
    private bool m_TurnLock = false;

    private Dictionary<ResourceType, int> m_Resources;
    private HexTile m_CurrentTile;

    public HexTile CurrentTile { get => m_CurrentTile; }
    public Dictionary<ResourceType, int> Resources { get => m_Resources; }

    public int PawnID { get => m_PawnID; }

    #region virtual MonoBehaviour functions

    public virtual void OnStart(){}

    #endregion

    void Awake()
    {   
        m_Resources = new Dictionary<ResourceType, int>();
        
        foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
        {
            if(!m_Resources.ContainsKey(resource) && resource != ResourceType.None)
            {
                m_Resources[resource] = 0;
            }
        }
    }

    void Start()
    {
        m_PawnID = TurnManager.s_Instance.AddPawn(this);
        
        m_TurnLock = false;
        
        HexTile startTile = HexGrid.s_Instance.GetTile(m_StartPosition);
        transform.position = startTile.transform.position;
        m_CurrentTile = startTile;
        m_CurrentTile.SetFog(false);
        var coords = HexGrid.s_Instance.NeighborTileCoords(m_CurrentTile.OffsetCoordinate);
        foreach (var coord in coords)
        {
            HexGrid.s_Instance.GetTile(coord).SetFog(false);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            ConstructBuidling(BuildingType.Town);
        }
    }

    protected IEnumerator JumpToTile(Vector3 start, Vector3 end, float height, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Lerp for horizontal movement
            Vector3 horizontalPosition = Vector3.Lerp(start, end, t);

            // Parabola for vertical movement
            float arc = height * Mathf.Sin(Mathf.PI * t); 

            // Apply new position
            transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + arc, horizontalPosition.z);

            yield return null;
        }
        
        transform.position = end;
        
    }
    
    public void TakeTurn(HexTile clickedTile)
    {
        if(m_TurnLock) return;
        
        HexTile tileOnPath = HexGrid.s_Instance.MoveToTile(m_CurrentTile, clickedTile);
        if(tileOnPath){
            StartCoroutine(JumpToTile(m_CurrentTile.transform.position, tileOnPath.transform.position, 1.0f, 0.2f));
            m_CurrentTile.Pawn = null;
            m_CurrentTile = tileOnPath;
            tileOnPath.Pawn = this;
        }
        
        m_CurrentTile.SetFog(false);
        var coords = HexGrid.s_Instance.NeighborTileCoords(m_CurrentTile.OffsetCoordinate);
        foreach (var coord in coords)
        {
            HexTile tile = HexGrid.s_Instance.GetTile(coord);
            tile.SetFog(false);
        }
        
        m_TurnLock = true;
        
        OnTurnTaken?.Invoke(PawnID);
    }
    
    public void ObtainResource(ResourceType resourceType, int amount)
    {
        Debug.Log(amount);
        if(m_Resources.ContainsKey(resourceType))
        {
            m_Resources[resourceType] += amount;
            ResorceChangeEvent?.Invoke(resourceType, m_Resources[resourceType]);
        }
    }
    
    public void TurnDoneCallback()
    {
        m_TurnLock = false;
    }
    
    public HexTile GetCurrentTile()
    {
        return m_CurrentTile;
    }

    public void SetVisible(bool visibility)
    {
    }
    
    
    public void MoveToTile(int tileID)
    {
        
    }
    
    public void ConstructBuidling(BuildingType buildingType)
    {
        PropertyManager.ConstructBuilding(buildingType, this, m_CurrentTile.TileId);
    }
    
    public void Attack()
    {
        
    }

}
