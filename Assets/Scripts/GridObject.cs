using UnityEngine;

public abstract class GridObject : MonoBehaviour
{
    void Awake()
    {
        OnAwake();    
    }

    void Start()
    {
        OnStart();
    }

    public virtual void OnAwake()
    {
    }

    public virtual void OnStart()
    {
        TurnManager.s_Instance.AddGridObject(this);
    }
    
    public abstract void StartTurn(HexTile clickedTile);
    public abstract void EndTurn(HexTile clickedTile);
    
}
