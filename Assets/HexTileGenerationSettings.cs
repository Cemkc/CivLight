using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/HexTileSettings", fileName = "HexTileSettings")]
public class HexTileGenerationSettings : ScriptableObject
{    
    public GameObject oceanTile;
    public GameObject plainsTile;
    public GameObject hillTile;
    
    public GameObject GetTilePrefab(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Plain:
                return plainsTile;
            case TileType.Ocean:
                return oceanTile;
            case TileType.Hill:
                return hillTile;
            default:
                return null;
        }
    }
    
}
