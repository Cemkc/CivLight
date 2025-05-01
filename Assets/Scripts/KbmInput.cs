using UnityEngine;

public interface IInputInvoker
{
    public void ConnectInput(IInputListener inputListener);
}

public interface IInputListener
{
    public void OnClickInput(Vector2 position);
}

public class KbmInput : MonoBehaviour, IInputInvoker
{
    [SerializeField] private bool m_AlternateInput = false;
    [SerializeField] private KeyCode m_AlternateKey = KeyCode.LeftControl;
    
    private InputEvents m_InputEvents;

    void Awake()
    {
        m_InputEvents = new InputEvents();
        m_AlternateKey = KeyCode.LeftControl;
    }

    void Update()
    {
        if(m_AlternateInput && !Input.GetKey(m_AlternateKey)) return; // Return for alternate input
        if(!m_AlternateInput && Input.GetKey(m_AlternateKey)) return; // Return for normal input
        
        if(Input.GetMouseButtonDown(0))
        {
            m_InputEvents.OnMouseClick?.Invoke(Input.mousePosition);
        }
    }
    
    public void ConnectInput(IInputListener inputListener)
    {
        m_InputEvents.OnMouseClick += inputListener.OnClickInput;
    }
}
