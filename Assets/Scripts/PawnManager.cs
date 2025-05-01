using System;
using UnityEngine;

public class PawnManager : MonoBehaviour, IInputListener
{
    [SerializeField] private Pawn m_Pawn;
    
    private PawnEvents m_PawnEvents;

    void Awake()
    {
    }

    void Onable()
    {
        ConnectPawn(m_Pawn);
    }

    void ConnectPawn(Pawn pawn)
    {
        if(pawn)
        {
            m_PawnEvents.MoveToTile += pawn.MoveToTile;
            m_PawnEvents.ConstructBuilding += pawn.ConstructBuidling;
        }
        
        m_PawnEvents.MoveToTile += pawn.MoveToTile;
        m_PawnEvents.ConstructBuilding += pawn.ConstructBuidling;
        
        m_Pawn = pawn;
    }

    public void OnClickInput(Vector2 position)
    {
        Vector2Int hexCoord = HexGrid.s_Instance.ScreenToHexCoord(position);
        HexTile tile = HexGrid.s_Instance.GetTile(hexCoord);
        
        if(!tile) return;
        
        m_Pawn.TakeTurn(tile);
    }
}
