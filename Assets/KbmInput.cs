using UnityEngine;
using UnityEngine.Events;

public interface IInputInvoker
{
    public void ConnectInput(IInputListener listener);
}

public interface IInputListener
{
    public void OnClickInput(Vector2 position);
}

public class KbmInput : MonoBehaviour, IInputInvoker
{
    
    UnityAction<Vector2> MouseCLick;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            MouseCLick?.Invoke(Input.mousePosition);
        }
    }
    
    public void ConnectInput(IInputListener listener)
    {
        MouseCLick += listener.OnClickInput;
    }
    
}
