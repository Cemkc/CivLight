using System.Collections.Generic;
using UnityEngine;

public struct Face
{
    public List<Vector3> vertices {get; private set;}
    public List <int> triangles {get; private set;}
    public List<Vector2> uvs {get; private set;}

    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HexRenderer : MonoBehaviour
{
    public float InnerRadius = 0.0f;
    public float TopRadius = 1.0f;
    public float BottomRadius = 1.0f;
    public float Height;
    public bool PointyTop = true;
    public Material TileMaterial;

    private Mesh m_Mesh;
    private MeshFilter m_MeshFilter;
    private MeshRenderer m_MeshRenderer;

    private List<Face> m_Faces;

    public void SetPointyTop(bool pointy)
    {
        PointyTop = pointy;
    }
    
    public void SetSize(float size)
    {
        BottomRadius = size;
    }
    
    private void Awake()
    {
        m_MeshFilter = GetComponent<MeshFilter>();
        m_MeshRenderer = GetComponent<MeshRenderer>();

        m_Mesh = new Mesh();
        m_Mesh.name = "Hex";

        m_MeshFilter.mesh = m_Mesh;
        m_MeshRenderer.material = TileMaterial;
    }
    
    private void OnEnable()
    {
        DrawMesh();
    }

    public void OnValidate()
    {
        if(Application.isPlaying)
        {
            DrawMesh();
        }
    }

    public void DrawMesh()
    {
        DrawFaces();
        CombineFaces();
    }

    private void DrawFaces()
    {
        m_Faces = new List<Face>();
        
        // Top Faces
        for(int i = 0; i < 6; i++)
        {
            m_Faces.Add(CreateFace(InnerRadius, TopRadius, Height / 2f, Height / 2f, i));
        }
        
        // Bottom Faces
        for(int i = 0; i < 6; i++)
        {
            m_Faces.Add(CreateFace(InnerRadius, BottomRadius, 0.0f, 0.0f, i, true));
        }
        
        // Inner Walls
        for(int i = 0; i < 6; i++)
        {
            m_Faces.Add(CreateFace(InnerRadius, InnerRadius, Height / 2f, 0.0f, i));
        }
        
        // Outer Walls
        for(int i = 0; i < 6; i++)
        {
            m_Faces.Add(CreateFace(BottomRadius, TopRadius, Height / 2f, 0.0f, i, true));
        }
        
    }
    
    private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point, bool reverse = false)
    {
        Vector3 pointA = GetPoint(innerRad, heightB, point);
        Vector3 pointB = GetPoint(innerRad, heightB, (point < 5) ? point + 1 : 0);
        Vector3 pointC = GetPoint(outerRad, heightA, (point < 5) ? point + 1 : 0);
        Vector3 pointD = GetPoint(outerRad, heightA, point);

        List<Vector3> vertices = new List<Vector3>() { pointA, pointB, pointC, pointD };
        List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
        List<Vector2> uvs = new List<Vector2>() { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        
        if (reverse)
            vertices.Reverse();

        return new Face(vertices, triangles, uvs);
    }
    
    private Vector3 GetPoint(float size, float height, int index)
    {
        float angle_deg = PointyTop ? 60 * index - 30 : 60 * index;
        float angle_rad = Mathf.PI / 180f * angle_deg;
        return new Vector3(size * Mathf.Cos(angle_rad), height, size * Mathf.Sin(angle_rad));
    }

    private void CombineFaces()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < m_Faces.Count; i++)
        {
            vertices.AddRange(m_Faces[i].vertices);
            uvs.AddRange(m_Faces[i].uvs);

            int offset = 4 * i;
            foreach (int triangle in m_Faces[i].triangles)
            {
                tris.Add(triangle + offset);
            }
        }

        m_Mesh.vertices = vertices.ToArray();
        m_Mesh.triangles = tris.ToArray();
        m_Mesh.uv = uvs.ToArray();
        m_Mesh.RecalculateNormals();
    }
}
