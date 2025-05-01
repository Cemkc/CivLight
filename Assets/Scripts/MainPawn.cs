// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Events;

// public class MainPawn : Pawn
// {
//     [SerializeField] Vector2Int m_StartPosition;

//     public UnityAction<ResourceType, int> ResorceChangeEvent;

//     private Dictionary<ResourceType, int> m_Resources;
//     private HexTile m_CurrentTile;

//     public HexTile CurrentTile { get => m_CurrentTile; }
//     public Dictionary<ResourceType, int> Resources { get => m_Resources; }
    
//     public void Awake()
//     {
//         Debug.Log(transform.name);
        
//         m_Resources = new Dictionary<ResourceType, int>();
        
//         foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
//         {
//             if(!m_Resources.ContainsKey(resource) && resource != ResourceType.None)
//             {
//                 m_Resources[resource] = 0;
//                 Debug.Log(resource);
//             }
//         }
//     }

//     public override void OnStart()
//     {
//         base.OnStart();
//         HexTile startTile = HexGrid.s_Instance.GetTile(m_StartPosition);
//         transform.position = startTile.transform.position;
//         m_CurrentTile = startTile;
//         m_CurrentTile.SetFog(false);
//         var coords = HexGrid.s_Instance.NeighborTileCoords(m_CurrentTile.OffsetCoordinate);
//         foreach (var coord in coords)
//         {
//             HexGrid.s_Instance.GetTile(coord).SetFog(false);
//         }
//     }
    
//     public void TakeTurn(HexTile clickedTile)
//     {
//         HexTile tileOnPath = HexGrid.s_Instance.MoveToTile(m_CurrentTile, clickedTile);
//         if(tileOnPath){
//             StartCoroutine(JumpToTile(m_CurrentTile.transform.position, tileOnPath.transform.position, 1.0f, 0.2f));
//             m_CurrentTile.Pawn = null;
//             m_CurrentTile = tileOnPath;
//             tileOnPath.Pawn = this;
//         }
        
//         m_CurrentTile.SetFog(false);
//         var coords = HexGrid.s_Instance.NeighborTileCoords(m_CurrentTile.OffsetCoordinate);
//         foreach (var coord in coords)
//         {
//             HexTile tile = HexGrid.s_Instance.GetTile(coord);
//             tile.SetFog(false);
//             var resource = tile.HarvestResource(1);
//             EditResource(resource.type, resource.givenAmount);
//         }
//     }
    
//     public override void EditResource(ResourceType resource, int amount)
//     {
//         if(m_Resources.ContainsKey(resource))
//         {
//             m_Resources[resource] += amount;
//             ResorceChangeEvent?.Invoke(resource, m_Resources[resource]);
//         }
//     }
    
//     public override HexTile GetCurrentTile()
//     {
//         return m_CurrentTile;
//     }

//     public override void SetVisible(bool visibility)
//     {
//     }
// }
