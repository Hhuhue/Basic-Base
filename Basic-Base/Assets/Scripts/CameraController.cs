// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
// for using the mouse displacement for calculating the amount of camera movement and panning code.

using UnityEngine;
using System.Collections;
using Border = Land.Border;

public class CameraController : MonoBehaviour
{
    public LandController Land;

    private Vector3 cameraPosition;
    private Border viewBorders;
    private const int maxSize = 10;
    private const int minSize = 2;
    private float cameraSize;

    void Start()
    {
        cameraSize = Camera.main.orthographicSize;

        UpdateViewBorders();
    }

    void Update()
    {
        cameraSize = Camera.main.orthographicSize;

        cameraPosition = Camera.main.transform.position;

        if (Input.anyKey)
        {
            switch (States.View)
            {
                case CameraView.MAP: MoveInMap(); break;

                case CameraView.LAND: MoveInLand(); break;
            }
        }

        ManageZoom();
    }

    private void MoveInMap()
    {
        Vector3 move = Vector3.zero;
        float panSpeed = cameraSize * 0.1f;

        if (Input.GetKey(KeyCode.W)) move.y = panSpeed;
        if (Input.GetKey(KeyCode.S)) move.y = -panSpeed;
        if (Input.GetKey(KeyCode.A)) move.x = -panSpeed;
        if (Input.GetKey(KeyCode.D)) move.x = panSpeed;

        if (!IsCameraInMapWidth(move)) move.x = move.x > 0
                ? move.x - (viewBorders.Right + move.x - Config.LandWidth)
                : move.x - (viewBorders.Left + move.x);

        if (!IsCameraInMapHeight(move)) move.y = move.y > 0
                ? move.y - (viewBorders.Top + move.y - Config.LandHeight)
                : move.y - (viewBorders.Bottom + move.y);

        Camera.main.transform.position = cameraPosition + move;
        cameraPosition += move;
        UpdateViewBorders();
    }

    private void MoveInLand()
    {
        Vector3 move = Vector3.zero;
        float panSpeed = cameraSize * 0.1f;

        if (Input.GetKey(KeyCode.W)) move.y = panSpeed;
        if (Input.GetKey(KeyCode.S)) move.y = -panSpeed;
        if (Input.GetKey(KeyCode.A)) move.x = -panSpeed;
        if (Input.GetKey(KeyCode.D)) move.x = panSpeed;

        if (!IsCameraInMapWidth(move))
        {
            move.x = move.x > 0
                ? -viewBorders.Left
                : Config.LandWidth - viewBorders.Right;

            Land.DrawLand(Land.RelativeBottomLeft + new Vector2(move.x, 0));
        }

        if (!IsCameraInMapHeight(move))
        {
            move.y = move.y > 0
                ? -viewBorders.Bottom
                : Config.LandHeight - viewBorders.Top;

            Land.DrawLand(Land.RelativeBottomLeft + new Vector2(0, move.y));
        }

        Camera.main.transform.position = cameraPosition + move;
        cameraPosition += move;
        UpdateViewBorders();
    }

    private void ManageZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // zoom back
        {
            if (Camera.main.orthographicSize < maxSize)
            {
                Camera.main.orthographicSize++;
                UpdateViewBorders();
                RelocateCamera();
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // zoom in
        {
            if (Camera.main.orthographicSize > minSize)
            {
                Camera.main.orthographicSize--;
                UpdateViewBorders();
            }
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

        if (cameraPosition.x + cameraSize * 2 >= Config.MapWidth * multiplier)
            SetPosition(new Vector3(Config.MapWidth - cameraSize * 2, cameraPosition.y, cameraPosition.z));

        if (cameraPosition.y - cameraSize < 0)
            SetPosition(Camera.main.transform.position = new Vector3(cameraPosition.x, cameraSize, cameraPosition.z));

        if (cameraPosition.y + cameraSize >= Config.MapHeight * multiplier)
            SetPosition(Camera.main.transform.position = new Vector3(cameraPosition.x, Config.MapHeight - cameraSize, cameraPosition.z));
    }

    private bool IsCameraInMapWidth(Vector3 move)
    {
        float cameraSize = Camera.main.orthographicSize * 2;

        int currentWidth = (States.View == CameraView.LAND) ? Config.LandWidth : Config.MapWidth;

        return cameraPosition.x - cameraSize + move.x >= 0 &&
               cameraPosition.x + cameraSize + move.x < currentWidth;
    }

    private bool IsCameraInMapHeight(Vector3 move)
    {
        float cameraSize = Camera.main.orthographicSize;

        int currentHeight = (States.View == CameraView.LAND) ? Config.LandHeight : Config.MapHeight;

        return cameraPosition.y - cameraSize + move.y >= 0 &&
               cameraPosition.y + cameraSize + move.y < currentHeight;
    }

    private void UpdateViewBorders()
    {
        cameraSize = Camera.main.orthographicSize;

        viewBorders = new Border
        {
            Bottom = cameraPosition.y - cameraSize,
            Top = cameraPosition.y + cameraSize,
            Left = cameraPosition.x - cameraSize * 2,
            Right = cameraPosition.x + cameraSize * 2
        };
    }

    public enum CameraView
    {
        MAP,
        LAND
    }
}