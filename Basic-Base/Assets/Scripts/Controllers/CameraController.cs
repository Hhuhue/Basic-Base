using Assets.Scripts.Models;
using UnityEngine;

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
        float currentSize = _thisCamera.orthographicSize;
        Vector3 currentPosition = _thisCamera.transform.position;

        Vector3 move = GetMove();

        if (currentPosition.x + currentSize * 2 + move.x > Config.ViewWidth)
            move.x = Config.ViewWidth - currentPosition.x - currentSize * 2;
        else if (currentPosition.x - currentSize * 2 + move.x < 0)
            move.x = 0 - currentPosition.x + currentSize * 2;

        if (currentPosition.y + currentSize + move.y > Config.ViewHeight)
            move.y = Config.ViewHeight - currentPosition.y - currentSize;
        else if (currentPosition.y - currentSize + move.y < 0)
            move.y = 0 - currentPosition.y + currentSize;

        _thisCamera.transform.position += move;
    }

    private void ManageZoom()
    {
        float currentSize = _thisCamera.orthographicSize;

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && currentSize - 1 >= 1)
            _thisCamera.orthographicSize--;

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && currentSize + 1 <= _maxSize)
            _thisCamera.orthographicSize++;
    }

    private Vector3 GetMove()
    {
        Vector3 move = Vector3.zero;

        float speed = _thisCamera.orthographicSize / 10;

        if (Input.GetKey(KeyCode.A)) move.x = -speed;
        if (Input.GetKey(KeyCode.D)) move.x = speed;
        if (Input.GetKey(KeyCode.S)) move.y = -speed;
        if (Input.GetKey(KeyCode.W)) move.y = speed;

        return move;
    }
}