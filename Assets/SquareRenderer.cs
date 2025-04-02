// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public struct Face
// {
//     public List<Vector3> vertices {get; private set;}
//     public List <int> triangles {get; private set;}
//     public List<Vector2> uvs {get; private set;}

//     public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
//     {
//         this.vertices = vertices;
//         this.triangles = triangles;
//         this.uvs = uvs;
//     }
// }

// [RequireComponent(typeof(MeshFilter))]
// [RequireComponent(typeof(MeshRenderer))]
// public class SquareRenderer : MonoBehaviour
// {
//     public bool PointyTop = true;

//     private MeshRenderer m_MeshRenderer;
//     private MeshFilter m_MeshFilter;

//     void Awake()
//     {
//         m_MeshRenderer = GetComponent<MeshRenderer>();
//         m_MeshFilter = GetComponent<MeshFilter>();
//     }

//     void Start()
//     {
//         Mesh mesh = new Mesh();
//         mesh.vertices = HexVertices(1.0f, 0.0f);
//         mesh.triangles = HexTriangles(true).ToArray();
        
//         m_MeshFilter.mesh = mesh;
        
//     }
    
//     private Face CreateFace(float radius, float heightA, float heightB)
//     {
//         List<Vector3> vertices = new List<Vector3>();
        
//         Vector3[] edges = new Vector3[7];
//         edges[0] = new Vector3(0, height, 0);
//         float angle_deg = PointyTop ? 30 : 0;
//         float angle_rad = Mathf.PI / 180f * angle_deg;
        
//         for(int i = 1; i <= 6; i++)
//         {
//             Debug.Log(Mathf.Rad2Deg * angle_rad);
//             edges[i] = new Vector3(size * Mathf.Cos(angle_rad), height, size * Mathf.Sin(angle_rad));
//             angle_rad += MathF.PI / 180f * 60f;
            
//         }
        
//         foreach (Vector3 edge in edges)
//         {
//             GameObject go = new GameObject();
//             go.transform.position = edge;
//             Debug.Log(edge);
//         }
        
//         return edges;
//     }
    
//     private Vector3 GetPoint(float size, float height, int index)
//     {
//         float angle_deg = PointyTop ? 60 * index - 30 : 60 * index;
//         float angle_rad = Mathf.PI / 180f * angle_deg;
//         return new Vector3(size * Mathf.Cos(angle_rad), height, size * Mathf.Sin(angle_rad));
//     }
    
//     private List<int> HexTriangles(bool reverse)
//     {
//         List<int> triangles = new List<int>();
        
//         for(int i = 1 ; i <= 6; i++)
//         {
//             triangles.Add(0);
//             triangles.Add(i);
//             if(i != 6)
//                 triangles.Add(i + 1);
//             else
//                 triangles.Add(1);
//         }
        
//         if(reverse)
//             triangles.Reverse();
        
//         foreach (var item in triangles)
//         {
//             Debug.Log(item);
//         }
        
//         return triangles;
//     }
// }
