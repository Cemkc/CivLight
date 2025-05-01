using System;
using UnityEngine;

public class InputEvents
{
    public Action<Vector2> OnMouseClick;
}

public class PawnEvents
{
    public Action<int> MoveToTile;
    public Action<BuildingType> ConstructBuilding;
}
