using System.Collections;
using UnityEngine;

public abstract class Pawn : GridObject, IInputListener
{   
    public abstract void SetVisible(bool visibility);
    public abstract void EditResource(ResourceType resource, int amount);
    public abstract HexTile GetCurrentTile();
          
    protected IEnumerator JumpToTile(Vector3 start, Vector3 end, float height, float duration)
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

    public void OnClickInput(Vector2 position)
    {
        throw new System.NotImplementedException();
    }

    public void OnAlternateClickInput(Vector2 position)
    {
        throw new System.NotImplementedException();
    }
}
