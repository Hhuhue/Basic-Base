// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
// for using the mouse displacement for calculating the amount of camera movement and panning code.

using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public int mapHeight;
    public int mapWidth;
    public float panSpeed = 4.0f;       // Speed of the camera when being panned
    public float zoomSpeed = 4.0f;      // Speed of the camera going back and forth

    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private bool isPanning;     // Is the camera being panned?
    
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }
        
        if (!Input.GetMouseButton(1)) isPanning = false;
        
        if (isPanning)
        {
            float cameraSize = Camera.main.orthographicSize;
            Vector3 cameraPosition = Camera.main.transform.position;

            Vector3 position = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(position.x * panSpeed, position.y * panSpeed, 0);

            if (cameraPosition.x - cameraSize*2 + move.x < 0 || cameraPosition.x + cameraSize*2 + move.x >= mapWidth)
                move.x = 0;

            if(cameraPosition.y - cameraSize + move.y < 0 || cameraPosition.y + cameraSize + move.y >= mapHeight)
                move.y = 0;

            transform.Translate(move, Space.Self);

        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0) // zoom back
        {
            if(Camera.main.orthographicSize < 10) Camera.main.orthographicSize++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // zoom in
        {
            if (Camera.main.orthographicSize > 2) Camera.main.orthographicSize--;
        }
    }

    public void SetPosition(Vector3 position)
    {
        Camera.main.transform.position = position;
    }
}