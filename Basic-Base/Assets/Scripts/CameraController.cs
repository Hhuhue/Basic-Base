// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
// for using the mouse displacement for calculating the amount of camera movement and panning code.

using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public static bool isPanning;     // Is the camera being panned?
    public int mapHeight;
    public int mapWidth;
    public float panSpeed = 4.0f;       // Speed of the camera when being panned
    public float zoomSpeed = 4.0f;      // Speed of the camera going back and forth

    private Vector3 cameraPosition;
    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private int maxSize;

    void Update()
    {
        float cameraSize = Camera.main.orthographicSize;
        cameraPosition = Camera.main.transform.position;
        maxSize = (States.View == CameraView.LAND) ? 6 : 10;

        if(cameraSize > maxSize)
        {
            cameraSize = maxSize;
            Camera.main.orthographicSize = maxSize;
        }

        if (Input.GetMouseButtonDown(1))
        {
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }
        
        if (!Input.GetMouseButton(1)) isPanning = false;
        
        if (isPanning)
        {
            Vector3 position = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(position.x * panSpeed, position.y * panSpeed, 0);

            if (!IsCameraInMapWidth(move)) move.x = 0;

            if (!IsCameraInMapHeight(move)) move.y = 0;

            transform.Translate(move, Space.Self);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0) // zoom back
        {
            if (Camera.main.orthographicSize < maxSize)
            {
                Camera.main.orthographicSize++;
                RelocateCamera();
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // zoom in
        {
            if (Camera.main.orthographicSize > 2) Camera.main.orthographicSize--;
        }
    }

    public void SetPosition(Vector3 position)
    {
        if (States.View == CameraView.LAND)
        {
            position = new Vector3(position.x * 10, position.y * 10, position.z);
        }

        Camera.main.transform.position = position;
    }

    public void ChangeView()
    {
        States.View = (States.View == CameraView.LAND) ? CameraView.MAP : CameraView.LAND;
    }

    private void RelocateCamera()
    {
        float cameraSize = Camera.main.orthographicSize;
        int multiplier = (States.View == CameraView.LAND) ? 10 : 1;

        if (cameraPosition.x - cameraSize * 2 < 0)
            SetPosition(new Vector3(cameraSize * 2, cameraPosition.y, cameraPosition.z));

        if (cameraPosition.x + cameraSize * 2 >= mapWidth * multiplier)
            SetPosition(new Vector3(mapWidth - cameraSize * 2, cameraPosition.y, cameraPosition.z));

        if (cameraPosition.y - cameraSize < 0)
            SetPosition(Camera.main.transform.position = new Vector3(cameraPosition.x, cameraSize, cameraPosition.z));

        if (cameraPosition.y + cameraSize >= mapHeight * multiplier)
            SetPosition(Camera.main.transform.position = new Vector3(cameraPosition.x, mapHeight - cameraSize, cameraPosition.z));
    }

    private bool IsCameraInMapWidth(Vector3 move)
    {
        float cameraSize = Camera.main.orthographicSize * 2;

        int currentWidth = (States.View == CameraView.LAND) ? mapWidth * 10 : mapWidth;

        return cameraPosition.x - cameraSize + move.x >= 0 &&
               cameraPosition.x + cameraSize + move.x < currentWidth;
    }

    private bool IsCameraInMapHeight(Vector3 move)
    {
        float cameraSize = Camera.main.orthographicSize;

        int currentHeight = (States.View == CameraView.LAND) ? mapHeight * 10 : mapHeight;

        return cameraPosition.y - cameraSize + move.y >= 0 &&
               cameraPosition.y + cameraSize + move.y < currentHeight;
    }

    public enum CameraView
    {
        MAP,
        LAND
    }
}