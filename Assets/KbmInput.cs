using UnityEngine;
using UnityEngine.Events;

public interface IInputInvoker
{
    public void ConnectInput(IInputListener listener);
}

public interface IInputListener
{
    public void OnClickInput(Vector2 position);
    public void OnAlternateClickInput(Vector2 position);
}

public class KbmInput : MonoBehaviour, IInputInvoker
{
    
    UnityAction<Vector2> MouseCLick;
    UnityAction<Vector2> AlternateMouseCLick;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(!Input.GetKey(KeyCode.LeftControl))
                MouseCLick?.Invoke(Input.mousePosition);
            else
                AlternateMouseCLick?.Invoke(Input.mousePosition);
        }
    }
    
    public void ConnectInput(IInputListener listener)
    {
        MouseCLick += listener.OnClickInput;
        AlternateMouseCLick += listener.OnAlternateClickInput;
    }
    
}
