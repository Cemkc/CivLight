using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    private Mesh m_Mesh;

    private Vector3[] m_Vertices;
    private int[] m_Indices;

    void Start()
    {
        m_Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = m_Mesh;

        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        m_Vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(-0.5f, 0.5f, 0.0f),
            new Vector3(0.5f, 0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f)
        };
        
        m_Indices = new int[] {0, 1, 3, 1, 2, 3};
        
    }
    
    void UpdateMesh()
    {
        m_Mesh.Clear();
        
        m_Mesh.vertices = m_Vertices;
        m_Mesh.triangles = m_Indices;
    }

    void Update()
    {
        
    }
}
