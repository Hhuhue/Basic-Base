using System;
using UnityEngine;
using System.Collections;
using Border = Land.Border;
using ViewMode = View.ViewMode;

public class CameraController : MonoBehaviour
{
    private Camera _thisCamera;
    private const int _maxSize = 8;

    void Start()
    {
        _thisCamera = Camera.main;
        _thisCamera.orthographicSize = _maxSize;
    }

    void Update()
    {
        ManageMovement();

        ManageZoom();
    }

    private void ManageMovement()
    {
        float speed = 0.1f * _thisCamera.orthographicSize;

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.A)) move.x = -speed;
        if (Input.GetKey(KeyCode.D)) move.x = speed;
        if (Input.GetKey(KeyCode.S)) move.y = -speed;
        if (Input.GetKey(KeyCode.W)) move.y = speed;

        _thisCamera.transform.position += move;
    }

    private void ManageZoom()
    {
        float currentSize = _thisCamera.orthographicSize;

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && currentSize - 1 >= 1)
            _thisCamera.orthographicSize --;

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && currentSize + 1 <= _maxSize)
            _thisCamera.orthographicSize ++;
    }
}