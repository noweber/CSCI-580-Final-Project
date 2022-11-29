using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputManager : InputManager
{
    Vector2Int screen;
    float mousePositionOnRotateStart;
    //Events
    public static event MoveInputHandler OnMoveInput;
    public static event RotateInputHandler OnRotateInput;
    public static event ZoomInputHandler OnZoomInput;
    public static event ResetInputHandler OnResetInput;

    private void Awake()
    {
        screen = new Vector2Int(Screen.width, Screen.height);
    }

    private void Update()
    {
        Vector3 mp = Input.mousePosition;
        bool mouseValid = (mp.y <= screen.y * 1.05f && mp.y >= screen.y * -0.05f &&
            mp.x <= screen.x * 1.05f && mp.x >= screen.x * -0.05f);

        if (!mouseValid)
            return;

        //MOVEMENT
        if (mp.y > screen.y * .95f)
        {
            OnMoveInput?.Invoke(Vector3.forward);
        }
        if (mp.y < screen.y * .05f)
        {
            OnMoveInput?.Invoke(Vector3.back);
        }
        if (mp.x < screen.x * .05f)
        {
            OnMoveInput?.Invoke(Vector3.left);
        }
        if (mp.x > screen.x * .95f)
        {
            OnMoveInput?.Invoke(Vector3.right);
        }
        //ROTATE
        if(Input.GetMouseButtonDown(1))
        {
            mousePositionOnRotateStart = mp.x;
        }
        else if(Input.GetMouseButton(1))
        {
            if(mp.x < mousePositionOnRotateStart)
            {
                OnRotateInput?.Invoke(-1f);
            }
            if (mp.x > mousePositionOnRotateStart)
            {
                OnRotateInput?.Invoke(1f);
            }
        }
        //ZOOM
        if(Input.mouseScrollDelta.y > 0)
        {
            OnZoomInput?.Invoke(-3f);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            OnZoomInput?.Invoke(3f);
        }

        if(Input.GetKey(KeyCode.Mouse2))
        {
            OnResetInput?.Invoke(null);
        }
    }
}
