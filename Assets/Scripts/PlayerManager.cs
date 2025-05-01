using UnityEngine;

public class PlayerManager : MonoBehaviour, IInputListener
{
    [SerializeField] private PawnManager m_PawnManager;

    void Awake()
    {
    }

    void OnEnable()
    {
        if(!m_PawnManager) return;
        PossesPawn(m_PawnManager);
        ConnectInput();
    }
    
    void PossesPawn(PawnManager pawnManager)
    {
        m_PawnManager = pawnManager;
    }
    
    public void ConnectInput()
    {
        IInputInvoker[] invokers = transform.GetComponents<IInputInvoker>();
        
        foreach (var invoker in invokers)
        {
            invoker.ConnectInput(this);
        }
    }

    public void OnClickInput(Vector2 position)
    {
        m_PawnManager.OnClickInput(position);
    }
}
