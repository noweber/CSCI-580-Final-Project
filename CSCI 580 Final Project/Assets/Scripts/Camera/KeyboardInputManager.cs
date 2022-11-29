using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputManager : InputManager
{
    //Events
    public static event MoveInputHandler OnMoveInput;
    public static event RotateInputHandler OnRotateInput;
    public static event ZoomInputHandler OnZoomInput;
    public static event ResetInputHandler OnResetInput;

    private void Update()
    {
        //MOVEMENT
        if(Input.GetKey(KeyCode.W))
        {
            OnMoveInput?.Invoke(Vector3.forward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            OnMoveInput?.Invoke(Vector3.back);
        }
        if (Input.GetKey(KeyCode.A))
        {
            OnMoveInput?.Invoke(Vector3.left);
        }
        if (Input.GetKey(KeyCode.D))
        {
            OnMoveInput?.Invoke(Vector3.right);
        }

        //ROTATE
        if (Input.GetKey(KeyCode.Q))
        {
            OnRotateInput?.Invoke(-1f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            OnRotateInput?.Invoke(1f);
        }

        //ZOOM
        if (Input.GetKey(KeyCode.Z))
        {
            OnZoomInput?.Invoke(-1f);
        }
        if (Input.GetKey(KeyCode.X))
        {
            OnZoomInput?.Invoke(1f);
        }

        //RESET
        if(Input.GetKey(KeyCode.Space))
        {
            OnResetInput?.Invoke(null);
        }
    }
}
