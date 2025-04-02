using UnityEngine;

[CreateAssetMenu(menuName = "Hex Tile", fileName = "Default Tile")]
public class HexTileSO : ScriptableObject
{
    public float InnerRadius = 0.0f;
    public float TopRadius = 1.0f;
    public float BottomRadius = 1.0f;
    public float Height;
    public bool PointyTop = true;
    public Material material;
}
