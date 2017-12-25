using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    /// <summary>
    /// The camera view controller
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        private Camera _thisCamera;
        private const int MAX_SIZE = 8;

        /// <summary>
        /// The start event
        /// </summary>
        void Start()
        {
            _thisCamera = Camera.main;
            _thisCamera.orthographicSize = MAX_SIZE;
        }

        /// <summary>
        /// Ther update event
        /// </summary>
        void Update()
        {
            manageMovement();

            manageZoom();
        }

        /// <summary>
        /// Manages the movement of the camera.
        /// </summary>
        private void manageMovement()
        {
            float currentSize = _thisCamera.orthographicSize;
            Vector3 currentPosition = _thisCamera.transform.position;

            Vector3 move = getMove();

            //If the camera right border will go pass the field of view 
            if (currentPosition.x + currentSize*2 + move.x > View.VIEW_WIDTH)
            {
                //Put the camera to the left side of the view
                move.x = View.VIEW_WIDTH - currentPosition.x - currentSize * 2;
            }
            //If the camera left border will go pass the field of view 
            else if (currentPosition.x - currentSize*2 + move.x < 0)
            {
                //Put the camera to the right side of the view
                move.x = 0 - currentPosition.x + currentSize * 2;
            }

            //If the camera top border will go pass the field of view 
            if (currentPosition.y + currentSize + move.y > View.VIEW_HEIGHT)
            {
                //Put the camera to the bottom side of the view
                move.y = View.VIEW_HEIGHT - currentPosition.y - currentSize;
            }
            //If the camera bottom border will go pass the field of view 
            else if (currentPosition.y - currentSize + move.y < 0)
            {
                //Put the camera to the top side of the view
                move.y = 0 - currentPosition.y + currentSize;
            }

            _thisCamera.transform.position += move;
        }

        /// <summary>
        /// Manages the zoom input of the user
        /// </summary>
        private void manageZoom()
        {
            float currentSize = _thisCamera.orthographicSize;

            //Manage zoom in
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && currentSize - 1 >= 1)
                _thisCamera.orthographicSize--;

            //Manage zoom out
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && currentSize + 1 <= MAX_SIZE)
                _thisCamera.orthographicSize++;
        }

        /// <summary>
        /// Makes a translation vector out of the user's key input
        /// </summary>
        /// <returns>The translation vector. </returns>
        private Vector3 getMove()
        {
            Vector3 move = Vector3.zero;

            //The speed is a tenth of the camera size
            float speed = _thisCamera.orthographicSize / 10;

            //Parse horizontal inputs
            if (Input.GetKey(KeyCode.A)) move.x = -speed;
            if (Input.GetKey(KeyCode.D)) move.x = speed;

            //Parse vertical inputs
            if (Input.GetKey(KeyCode.S)) move.y = -speed;
            if (Input.GetKey(KeyCode.W)) move.y = speed;

            return move;
        }
    }
}