// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Events;

// public class AlternatePlayer : Pawn
// {
//     [SerializeField] Vector2Int m_StartPosition;
//     [SerializeField] GameObject m_Visuals;

//     public UnityAction<ResourceType, int> ResorceChangeEvent;

//     private Dictionary<ResourceType, int> m_Resources;
//     private HexTile m_CurrentTile;

//     public HexTile CurrentTile { get => m_CurrentTile; }
//     public Dictionary<ResourceType, int> Resources { get => m_Resources; }
    
//     public override void OnAwake()
//     {
//         base.OnAwake();
//         Debug.Log(transform.name);
        
//         m_Resources = new Dictionary<ResourceType, int>();
        
//         foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
//         {
//             if(!m_Resources.ContainsKey(resource))
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
//     }
    
//     public override void StartTurn(HexTile clickedTile)
//     {
//         HexTile tileOnPath = HexGrid.s_Instance.MoveToTile(m_CurrentTile, clickedTile);
//         if(tileOnPath){
//             StartCoroutine(JumpToTile(m_CurrentTile.transform.position, tileOnPath.transform.position, 1.0f, 0.2f));
//             m_CurrentTile.Pawn = null;
//             m_CurrentTile = tileOnPath;
//             m_CurrentTile.Pawn = this;
//         }
//     }

//     public override void EndTurn(HexTile clickedTile)
//     {
//     }
    
//     public override void EditResource(ResourceType resource, int amount)
//     {
//         if(m_Resources.ContainsKey(resource))
//         {
//             m_Resources[resource] += amount;
//             ResorceChangeEvent?.Invoke(resource, m_Resources[resource]);
//         }
//     }

//     public override void SetVisible(bool visibility)
//     {
//         if(visibility)
//             m_Visuals.gameObject.layer = LayerMask.NameToLayer("Default");
//         else
//             m_Visuals.gameObject.layer = LayerMask.NameToLayer("Hidden");
//     }

//     public override HexTile GetCurrentTile()
//     {
//         return m_CurrentTile;
//     }
// }
