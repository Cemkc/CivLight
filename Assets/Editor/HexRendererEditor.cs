using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexRenderer))]
public class HexRendererEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        HexRenderer renderer = (HexRenderer)target;
        
        if(GUILayout.Button("Render Tile"))
        {
            renderer.DrawMesh();
        }
    }
}
