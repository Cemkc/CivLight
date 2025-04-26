using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/HexTileRenderSettings", fileName = "HexTileRenderSettings")]
public class HexTileRenderSettings : ScriptableObject
{    
    public float InnerRadius = 0.0f;
    public float TopRadius = 1.0f;
    public float BottomRadius = 1.0f;
    public float Height = 0.0f;
    public bool PointyTop = true;
    public Material TileMaterial;
    
}