using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Cam Positioning")]
    public Vector2 camOffset = new Vector2(10f,14f);
    public float lookAtOffset = 2f;

    [Header("Move Controls")]
    public float inOutSpeed = 5f;
    public float lateralSpeed = 5f;
    public float rotateSpeed = 45f;

    [Header("Move Bounds")]
    public Vector2 minBounds, maxBounds;

    [Header("Zoom Controls")]
    public float zoomSpeed = 4f;
    public float zoomNearLimit = 2f;
    public float zoomFarLimit = 16f;
    public float startZoom = 5f;

    IZoomStrategy zoomStrategy;
    Vector3 frameMove;
    float frameRotate;
    float frameZoom;
    Camera cam;

    Vector3 startPos;

    private void Awake()
    {
        //InitCameraPos();
    }
    private void OnEnable()
    {
        KeyboardInputManager.OnMoveInput += UpdateFrameMove;
        KeyboardInputManager.OnRotateInput += UpdateFrameRotate;
        KeyboardInputManager.OnZoomInput += UpdateFrameZoom;
        KeyboardInputManager.OnResetInput += InitCameraPos;
        //MouseInputManager.OnMoveInput += UpdateFrameMove;
        //MouseInputManager.OnRotateInput += UpdateFrameRotate;
        MouseInputManager.OnZoomInput += UpdateFrameZoom;
        MouseInputManager.OnResetInput += InitCameraPos;
    }
    private void OnDisable()
    {
        KeyboardInputManager.OnMoveInput -= UpdateFrameMove;
        KeyboardInputManager.OnRotateInput -= UpdateFrameRotate;
        KeyboardInputManager.OnZoomInput -= UpdateFrameZoom;
        KeyboardInputManager.OnResetInput -= InitCameraPos;
        //MouseInputManager.OnMoveInput -= UpdateFrameMove;
        //MouseInputManager.OnRotateInput -= UpdateFrameRotate;
        MouseInputManager.OnZoomInput -= UpdateFrameZoom;
        MouseInputManager.OnResetInput -= InitCameraPos;
    }

    public void InitCameraPos(Vector3? initPos = null)
    {
        if(initPos.HasValue)
        {
            startPos = initPos.Value;
        }

        if(startPos != null)
        {
            this.transform.localPosition = startPos;
        }

        cam = GetComponentInChildren<Camera>();
        cam.transform.localPosition = new Vector3(0, Mathf.Abs(camOffset.y), -Mathf.Abs(camOffset.x));
        if (cam.orthographic)
            zoomStrategy = new OrthographicZoomStrategy(cam, startZoom);
        else
            zoomStrategy = new PerspectiveZoomStrategy(cam, camOffset, startZoom);
        cam.transform.LookAt(transform.position + Vector3.up * lookAtOffset);
    }

    private void UpdateFrameMove(Vector3 moveVector)
    {
        frameMove += moveVector;
    }

    private void UpdateFrameRotate(float rotateAmt)
    {
        frameRotate += rotateAmt;
    }

    private void UpdateFrameZoom(float zoomAmt)
    {
        frameZoom += zoomAmt;
    }
    private void LateUpdate()
    {
        if(frameMove != Vector3.zero)
        {
            Vector3 speedModFrameMove = new Vector3(frameMove.x * lateralSpeed, frameMove.y, frameMove.z * inOutSpeed);
            transform.position += transform.TransformDirection(speedModFrameMove) * Time.deltaTime;
            LockPositionInBounds();
            frameMove = Vector3.zero;
        }
        if(frameRotate != 0)
        {
            transform.Rotate(Vector3.up,frameRotate * rotateSpeed * Time.deltaTime);
            frameRotate = 0;
        }

        if (frameZoom < 0)
        {
            zoomStrategy.ZoomIn(cam,Time.deltaTime * Mathf.Abs(frameZoom) * zoomSpeed,zoomNearLimit);
            frameZoom = 0f;
        }
        if (frameZoom > 0)
        {
            zoomStrategy.ZoomOut(cam, Time.deltaTime * Mathf.Abs(frameZoom) * zoomSpeed,zoomFarLimit);
            frameZoom = 0f;
        }
    }

    private void LockPositionInBounds()
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x,minBounds.x,maxBounds.x),
            transform.position.y,
            Mathf.Clamp(transform.position.z, minBounds.y, maxBounds.y)
            );
    }
}
