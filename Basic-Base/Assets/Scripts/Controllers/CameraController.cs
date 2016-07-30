using UnityEngine;
using System.Collections;
using Border = Land.Border;
using ViewMode = View.ViewMode;

public class CameraController : MonoBehaviour
{
    public ViewController Land;

    private Vector3 _cameraPosition;
    private Border _viewBorders;
    private const int MaxSize = 10;
    private const int MinSize = 2;
    private float _cameraSize;

    void Start()
    {
        _cameraSize = Camera.main.orthographicSize;

        UpdateViewBorders();
    }

    void Update()
    {
        _cameraSize = Camera.main.orthographicSize;

        _cameraPosition = Camera.main.transform.position;

        MoveInLand();

        ManageZoom();
    }

    private void MoveInMap()
    {
        Vector3 move = Vector3.zero;
        float panSpeed = _cameraSize * 0.1f;

        if (Input.GetKey(KeyCode.W)) move.y = panSpeed;
        if (Input.GetKey(KeyCode.S)) move.y = -panSpeed;
        if (Input.GetKey(KeyCode.A)) move.x = -panSpeed;
        if (Input.GetKey(KeyCode.D)) move.x = panSpeed;

        if (!IsCameraInViewWidth(move))
            move.x = move.x > 0
                ? move.x - (_viewBorders.Right + move.x - Config.ViewWidth)
                : move.x - (_viewBorders.Left + move.x);

        if (!IsCameraInViewHeight(move))
            move.y = move.y > 0
                ? move.y - (_viewBorders.Top + move.y - Config.ViewHeight)
                : move.y - (_viewBorders.Bottom + move.y);

        Camera.main.transform.position = _cameraPosition + move;
        _cameraPosition += move;
        UpdateViewBorders();
    }

    private void MoveInLand()
    {
        Vector3 move = Vector3.zero;
        float panSpeed = _cameraSize * 0.1f;

        if (Input.GetKey(KeyCode.W)) move.y = panSpeed;
        if (Input.GetKey(KeyCode.S)) move.y = -panSpeed;
        if (Input.GetKey(KeyCode.A)) move.x = -panSpeed;
        if (Input.GetKey(KeyCode.D)) move.x = panSpeed;

        if (!IsCameraInViewWidth(move))
        {
            move.x = move.x > 0
                ? -_viewBorders.Left
                : Config.ViewWidth - _viewBorders.Right;

            Land.OnBorderReached(move.x > 0 ? Map.Orientation.RIGHT : Map.Orientation.LEFT);
        }

        if (!IsCameraInViewHeight(move))
        {
            move.y = move.y > 0
                ? -_viewBorders.Bottom
                : Config.ViewHeight - _viewBorders.Top;

            Land.OnBorderReached(move.y > 0 ? Map.Orientation.TOP : Map.Orientation.BOTTOM);
        }

        Camera.main.transform.position = _cameraPosition + move;
        _cameraPosition += move;
        RelocateCamera();
        UpdateViewBorders();
    }

    private void ManageZoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // zoom back
        {
            if (Camera.main.orthographicSize < MaxSize)
            {
                Camera.main.orthographicSize++;
                UpdateViewBorders();
                RelocateCamera();
            }
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0) // zoom in
        {
            if (Camera.main.orthographicSize > MinSize)
            {
                Camera.main.orthographicSize--;
                UpdateViewBorders();
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        Camera.main.transform.position = position;
        _cameraPosition = position;

        RelocateCamera();
    }

    public void ChangeView()
    {
        States.View = (States.View == ViewMode.LAND) ? ViewMode.MAP : ViewMode.LAND;
    }

    private void RelocateCamera()
    {
        float cameraSize = Camera.main.orthographicSize;
        int multiplier = (States.View == ViewMode.LAND) ? 10 : 1;

        if (_cameraPosition.x - cameraSize * 2 < 0)
            SetPosition(new Vector3(cameraSize * 2, _cameraPosition.y, _cameraPosition.z));

        if (_cameraPosition.x + cameraSize * 2 >= Config.MapWidth * multiplier)
            SetPosition(new Vector3(Config.MapWidth - cameraSize * 2, _cameraPosition.y, _cameraPosition.z));

        if (_cameraPosition.y - cameraSize < 0)
            SetPosition(Camera.main.transform.position = new Vector3(_cameraPosition.x, cameraSize, _cameraPosition.z));

        if (_cameraPosition.y + cameraSize >= Config.MapHeight * multiplier)
            SetPosition(Camera.main.transform.position = new Vector3(_cameraPosition.x, Config.MapHeight - cameraSize, _cameraPosition.z));
    }

    private bool IsCameraInViewWidth(Vector3 move)
    {
        float cameraSize = Camera.main.orthographicSize * 2;

        int currentWidth = Config.ViewWidth;

        return _cameraPosition.x - cameraSize + move.x >= 0 &&
               _cameraPosition.x + cameraSize + move.x < currentWidth;
    }

    private bool IsCameraInViewHeight(Vector3 move)
    {
        float cameraSize = Camera.main.orthographicSize;

        int currentHeight = Config.ViewHeight;

        return _cameraPosition.y - cameraSize + move.y >= 0 &&
               _cameraPosition.y + cameraSize + move.y < currentHeight;
    }

    private void UpdateViewBorders()
    {
        _cameraSize = Camera.main.orthographicSize;

        _viewBorders = new Border
        {
            Bottom = _cameraPosition.y - _cameraSize,
            Top = _cameraPosition.y + _cameraSize,
            Left = _cameraPosition.x - _cameraSize * 2,
            Right = _cameraPosition.x + _cameraSize * 2
        };
    }
}