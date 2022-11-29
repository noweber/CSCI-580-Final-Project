using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputManager : MonoBehaviour
{
    public delegate void MoveInputHandler(Vector3 moveVector);
    public delegate void RotateInputHandler(float rotateAmt);
    public delegate void ZoomInputHandler(float zoomAmt);
    public delegate void ResetInputHandler(Vector3? initPos);
}
