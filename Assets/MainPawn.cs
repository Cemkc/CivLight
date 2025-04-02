using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPawn : MonoBehaviour
{
    private HexTile m_CurrentTile;

    public HexTile CurrentTile { get => m_CurrentTile; }

    void Start()
    {
        HexGrid.s_Instance.OnTileClick += OnTileClick;
        
        HexTile startTile = HexGrid.s_Instance.GetTile(0);
        transform.position = startTile.transform.position;
        m_CurrentTile = startTile;
    }

    void OnTileClick(HexTile tile)
    {
        HexTile tileOnPath = HexGrid.s_Instance.MoveToTile(m_CurrentTile, tile);
        Debug.Log(tileOnPath);
        if(tileOnPath){
            StartCoroutine(JumpToTile(m_CurrentTile.transform.position, tileOnPath.transform.position, 1.0f, 0.2f));
            m_CurrentTile = tileOnPath;
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
