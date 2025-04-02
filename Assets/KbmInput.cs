using UnityEngine;
using UnityEngine.Events;

public interface IInputInvoker
{
    public void ConnectInput(HexGrid grid);
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
    
    public void ConnectInput(HexGrid grid)
    {
        MouseCLick += grid.OnClickInput;
    }
    
}
