using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private class PawnTracking
    {
        public Pawn Pawn;
        public bool TurnTaken;
    }
    
    public Action TurnDone;

    List<GridObject> gridObjects;
    Dictionary<int, PawnTracking> pawnTrackingDict;
    
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
        pawnTrackingDict = new Dictionary<int, PawnTracking>();
    }

    public void Start()
    {
        IInputInvoker[] invokers = GameObject.FindGameObjectWithTag("Input").transform.GetComponents<IInputInvoker>();
        
        foreach (var invoker in invokers)
        {
            // invoker.ConnectInput(this);
        }
    }

    public void AddGridObject(GridObject gridObject)
    {
        gridObjects.Add(gridObject);
    }
    
    public int AddPawn(Pawn pawn)
    {
        int id = pawnTrackingDict.Count;
        PawnTracking pawn1 = new PawnTracking
        {
            TurnTaken = false,
            Pawn = pawn
        };
        pawnTrackingDict.Add(id, pawn1);
        TurnDone += pawn.TurnDoneCallback;
        pawn.OnTurnTaken += OnPawnTurnTaken;
        return id;
    }
    
    public void OnPawnTurnTaken(int id)
    {
        pawnTrackingDict[id].TurnTaken = true;
        
        bool allPawnsTakenTurn = true;
        foreach (var pawn in pawnTrackingDict)
        {
            if(!pawn.Value.TurnTaken)
            {
                allPawnsTakenTurn = false;
                break;
            }
        }
        
        if(allPawnsTakenTurn)
        {
            foreach (var pawn in pawnTrackingDict)
            {
                pawn.Value.TurnTaken = false;
            }
            GridObjectsTakeTurn();
            TurnDone?.Invoke();
        }
    }
    
    public void GridObjectsTakeTurn()
    {
        foreach (GridObject gridObject in gridObjects)
        {
            gridObject.StartTurn();
        }
    
        foreach (GridObject gridObject in gridObjects)
        {
            gridObject.EndTurn();
        }
    }
}
