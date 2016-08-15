using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionController : MonoBehaviour
{
    private bool _isSelecting;
    private Vector3 _mousePosition1;

    void Start()
    {
    }

    void Update()
    {
        // If we press the left mouse button, save mouse location and begin selection
        if (Input.GetMouseButtonDown(0))
        {
            _isSelecting = true;
            _mousePosition1 = Input.mousePosition;
        }
        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0)) _isSelecting = false;

        if (Input.GetMouseButtonDown(1))
        {
            List<PersonController> selection = GetSelection();
            Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            foreach (PersonController controller in selection)
            {
                controller.GoToPosition(clickPosition);
                controller.SetSelected(false);
            }
        }
    }

    void OnGUI()
    {
        if (!_isSelecting) return;

        // Create a rect from both mouse positions
        var rect = RectangleDrawer.GetScreenRect(_mousePosition1, Input.mousePosition);
        RectangleDrawer.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
        RectangleDrawer.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform entity = transform.GetChild(i);

            entity.GetComponent<PersonController>().SetSelected(IsWithinSelectionBounds(entity));
        }
    }

    private List<PersonController> GetSelection()
    {
        List<PersonController> selection = new List<PersonController>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform entity = transform.GetChild(i);
            PersonController controller = entity.GetComponent<PersonController>();
            if(controller.Person.Selected) selection.Add(controller);
        }

        return selection;
    } 
    
    private bool IsWithinSelectionBounds(Transform entity)
    {
        if (!_isSelecting) return false;

        var mainCamera = Camera.main;
        var viewportBounds = RectangleDrawer.GetViewportBounds(mainCamera, _mousePosition1, Input.mousePosition);

        return viewportBounds.Contains(mainCamera.WorldToViewportPoint(entity.position));
    }
}
