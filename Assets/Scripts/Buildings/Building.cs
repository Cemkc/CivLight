using System;
using UnityEngine;

public enum BuildingType
{
    None,
    Town
}

public abstract class Building : GridObject
{
    protected BuildingType m_BuildingType;
    protected Pawn m_OwnerPawn;
    protected HexTile m_Tile;

    public BuildingType BuildingType { get => m_BuildingType; }
    public Pawn OwnerPawn { get => m_OwnerPawn; set => m_OwnerPawn = value; }
    public HexTile Tile { get => m_Tile; set => m_Tile = value; }

    public virtual void Init(Pawn ownerPawn, int tileID)
    {
        m_OwnerPawn = ownerPawn;
        m_Tile = HexGrid.s_Instance.GetTile(tileID);
        m_Tile.PossesBuilding(this);
    }
    
    public static Type GetType(BuildingType type)
    {
        switch (type)
        {
            case BuildingType.Town:
                return typeof(Town);
            default:
                throw new ArgumentException($"No class mapped for BuildingType {type}");
        }
    }
}

