using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/HexTileSettings", fileName = "HexTileSettings")]
public class HexTileGenerationSettings : ScriptableObject
{    
    public GameObject oceanTile;
    public GameObject plainsTile;
    
    public HexTileRenderSettings oceanRenderSettings;
    public HexTileRenderSettings plainsRenderSettings;
    
    public GameObject GetTilePrefab(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Plain:
                return plainsTile;
            case TileType.Ocean:
                return oceanTile;
            default:
                return null;
        }
    }
    
    public HexTileRenderSettings GetTileRenderSettings(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Plain:
                return plainsRenderSettings;
            case TileType.Ocean:
                return oceanRenderSettings;
            default:
                return null;
        }
    }
    
}
