using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainPawn : GridObject
{
    public UnityAction<ResourceType, int> ResorceChangeEvent;

    private Dictionary<ResourceType, int> m_Resources;
    private HexTile m_CurrentTile;

    public HexTile CurrentTile { get => m_CurrentTile; }
    public Dictionary<ResourceType, int> Resources { get => m_Resources; }
    
    public override void OnAwake()
    {
        base.OnAwake();
        Debug.Log(transform.name);
        
        m_Resources = new Dictionary<ResourceType, int>();
        
        foreach (ResourceType resource in Enum.GetValues(typeof(ResourceType)))
        {
            if(!m_Resources.ContainsKey(resource))
            {
                m_Resources[resource] = 0;
                Debug.Log(resource);
            }
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        HexTile startTile = HexGrid.s_Instance.GetTile(0);
        transform.position = startTile.transform.position;
        m_CurrentTile = startTile;
    }
    
    public override void StartTurn(HexTile clickedTile)
    {
        HexTile tileOnPath = HexGrid.s_Instance.MoveToTile(m_CurrentTile, clickedTile);
        if(tileOnPath){
            StartCoroutine(JumpToTile(m_CurrentTile.transform.position, tileOnPath.transform.position, 1.0f, 0.2f));
            m_CurrentTile.MainPawn = null;
            m_CurrentTile = tileOnPath;
            tileOnPath.MainPawn = this;
        }
    }

    public override void EndTurn(HexTile clickedTile)
    {
    }
    
    public void EditResource(ResourceType resource, int amount)
    {
        if(m_Resources.ContainsKey(resource))
        {
            m_Resources[resource] += amount;
            ResorceChangeEvent?.Invoke(resource, m_Resources[resource]);
        }
    }
          
    private IEnumerator JumpToTile(Vector3 start, Vector3 end, float height, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Lerp for horizontal movement
            Vector3 horizontalPosition = Vector3.Lerp(start, end, t);

            // Parabola for vertical movement
            float arc = height * Mathf.Sin(Mathf.PI * t); 

            // Apply new position
            transform.position = new Vector3(horizontalPosition.x, horizontalPosition.y + arc, horizontalPosition.z);

            yield return null;
        }
        
        transform.position = end;
        
    }
}
