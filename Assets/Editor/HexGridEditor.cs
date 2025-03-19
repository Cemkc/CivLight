using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        HexGrid hexGrid = (HexGrid)target;
        
        if(GUILayout.Button("Generate Grid"))
        {
            hexGrid.LayoutGrid();
        }
    }
}
