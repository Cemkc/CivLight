using UnityEngine;

public interface ICombatObject
{
    public void Attack(ICombatObject combatObject, int damage);
    public void TakeDamage(int damage);
    public Transform GetTransform();
}

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
    
    public abstract void StartTurn();
    public abstract void EndTurn();
    
}
