using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class TurnManager : MonoBehaviour, IInputListener
{
    List<GridObject> gridObjects;
    
    public static TurnManager s_Instance = null;

    public void Awake()
    {
        if(s_Instance)
        {
            Debug.LogError("Multiple instances of Turn Manager is not allowed.");
            Destroy(this);
        }
        else
        {
            s_Instance = this;
        }
        
        gridObjects = new List<GridObject>();
        
        Debug.Log("Turn Manager Instance: " + s_Instance);
    }

    public void Start()
    {
        IInputInvoker[] invokers = GameObject.FindGameObjectWithTag("Input").transform.GetComponents<IInputInvoker>();
        
        foreach (var invoker in invokers)
        {
            invoker.ConnectInput(this);
        }
    }

    public void Update() // For Debug
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            Pawn player = GameObject.Find("Player").GetComponent<Pawn>();
            PropertyManager.ConstructBuilding(BuildingType.Town, player, player.GetCurrentTile().TileId);
        }
    }

    public void AddGridObject(GridObject gridObject)
    {
        gridObjects.Add(gridObject);
    }
    
    public void OnClickInput(Vector2 position)
    {
        Vector2Int hexCoord = HexGrid.s_Instance.ScreenToHexCoord(position);
        HexTile tile = HexGrid.s_Instance.GetTile(hexCoord);
        
        if(!tile) return;

        foreach (GridObject gridObject in gridObjects)
        {
            if(gridObject.transform.name == "AlternatePlayer") continue; // For Debugging
            gridObject.StartTurn(tile);
        }
    
        foreach (GridObject gridObject in gridObjects)
        {
            if(gridObject.transform.name == "AlternatePlayer") continue;
            gridObject.EndTurn(tile);
        }
    }
    
    public void OnAlternateClickInput(Vector2 position)
    {
        Vector2Int hexCoord = HexGrid.s_Instance.ScreenToHexCoord(position);
        HexTile tile = HexGrid.s_Instance.GetTile(hexCoord);
        
        if(!tile) return;

        foreach (GridObject gridObject in gridObjects)
        {
            if(gridObject.transform.name == "Player") continue;
            gridObject.StartTurn(tile);
        }
    
        foreach (GridObject gridObject in gridObjects)
        {
            if(gridObject.transform.name == "Player") continue;
            gridObject.EndTurn(tile);
        }
    }
}
