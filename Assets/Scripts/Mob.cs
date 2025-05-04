using System.Collections;
using UnityEngine;

public enum MobType
{
    None,
    Spider,
    Dragon
}

public class Mob : GridObject, ICombatObject
{
    [SerializeField] private MobData m_MobData;
    private HexTile m_Tile;

    public HexTile Tile { get => m_Tile; }

    public void Init(HexTile tile)
    {
        m_Tile = tile;
    }

    public override void StartTurn()
    {
        var neighborTiles = HexGrid.s_Instance.NeighborTileCoords(m_Tile.OffsetCoordinate);
        foreach (Vector2Int tileCoord in neighborTiles)
        {
            HexTile tile = HexGrid.s_Instance.GetTile(tileCoord);
            if(tile.Pawn)
            {
                Attack(tile.Pawn, m_MobData.Damage);
            }
        }
    }
    
    public override void EndTurn()
    {
        
    }

    public void Attack(ICombatObject combatObject, int damage)
    {
        StartCoroutine(JumpToTileAndBack(transform.position, combatObject.GetTransform().position, 1.0f, 0.2f));
        combatObject.TakeDamage(m_MobData.Damage);
    }

    public void TakeDamage(int damage)
    {
        m_MobData.Health -= damage;
        if(m_MobData.Health <= 0)
        {
            m_MobData.Health = 0;
            Debug.Log(transform.name + " Died!");
        }
    }

    public Transform GetTransform()
    {
        return transform;
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
    
    protected IEnumerator JumpToTileAndBack(Vector3 start, Vector3 end, float height, float duration)
    {
        // Forward jump
        yield return StartCoroutine(JumpToTile(start, end, height, duration));

        // Backward jump
        yield return StartCoroutine(JumpToTile(end, start, height, duration));
    }
    
}
