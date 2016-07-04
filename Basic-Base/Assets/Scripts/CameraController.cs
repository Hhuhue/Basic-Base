// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
// for using the mouse displacement for calculating the amount of camera movement and panning code.

using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public int mapHeight;
    public int mapWidth;

    private Vector3 cameraPosition;
    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private int maxSize;

    void Update()
    {
        float cameraSize = Camera.main.orthographicSize;
        float panSpeed = cameraSize * 0.1f;

        cameraPosition = Camera.main.transform.position;
        maxSize = 10;

        if (Input.anyKey && States.View == CameraView.MAP)
        {
            Vector3 move = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) move.y = panSpeed;
            if (Input.GetKey(KeyCode.S)) move.y = -panSpeed;
            if (Input.GetKey(KeyCode.A)) move.x = -panSpeed;
            if (Input.GetKey(KeyCode.D)) move.x = panSpeed;

            if (!IsCameraInMapWidth(move)) move.x = 0;

            if (!IsCameraInMapHeight(move)) move.y = 0;

            Camera.main.transform.position = cameraPosition + move;

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
            position = new Vector3(position.x, position.y, position.z);
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