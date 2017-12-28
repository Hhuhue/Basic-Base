using Assets.Scripts.Models;
using Assets.Scripts.Models.Mapping;
using Assets.Scripts.Tools;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    /// <summary>
    /// Class managing a tile of the view 
    /// </summary>
    public class TileController : MonoBehaviour
    {
        private Tile _tile;
        private GameObject _icon;
        private TextMesh _textMesh;
        private ViewController _viewController;
        private Game _game;

        private int _xPosition;
        private int _yPosition;

        void Start()
        {
            _viewController = transform.parent.gameObject.GetComponent<ViewController>();
            _game = GameProvider.Game();
        }

        void OnMouseDown()
        {
            if (_game.GetViewMode() == View.ViewMode.MAP)
            {
                _game.ChangeViewMode(View.ViewMode.LAND, _tile);
            }
        }
    
        void OnMouseEnter()
        {
            if(_viewController == null) return;

            if (_game.GetViewMode() == View.ViewMode.LAND) displayPosition(true);

            _viewController.SelectTile(_xPosition, _yPosition);
        }

        void OnMouseExit()
        {
            if (_game.GetViewMode() == View.ViewMode.LAND) displayPosition(false);
        }

        /// <summary>
        /// Sets the position of the tile.
        /// </summary>
        /// <param name="x">The x coordinate. </param>
        /// <param name="y">The y coordinate. </param>
        public void SetPosition(int x, int y)
        {
            _xPosition = x;
            _yPosition = y;
        }

        /// <summary>
        /// Sets the model of the tile.
        /// </summary>
        /// <param name="tile">The tile model. </param>
        public void SetTile(Tile tile)
        {
            _tile = tile;

            string sprite = tile.Type.ToString().ToLower();

            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Game.SpritesPath + sprite);

            setIcon(tile);
        }

        /// <summary>
        /// Sets the icon of the tile.
        /// </summary>
        /// <param name="tile">The tile model. </param>
        private void setIcon(Tile tile)
        {
            string iconPath = tile.Icon == Tile.TileType.DEFAULT ? "" : tile.Icon.ToString().ToLower();

            _icon = transform.GetChild(0).gameObject;
            _icon.transform.position = new Vector3(transform.position.x, transform.position.y, _icon.transform.parent.position.z - 1);
            _icon.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(Game.SpritesPath + iconPath);
            _icon.transform.localEulerAngles = Tile.OrientationToVector(tile.Orientation);
        }

        /// <summary>
        /// Toggles the map position of the tile.
        /// </summary>
        /// <param name="display">Whether to hide or show the position. </param>
        private void displayPosition(bool display)
        {
            if (_textMesh == null)
            {
                GameObject textObject = new GameObject("Text");
                textObject.AddComponent<MeshRenderer>();
                textObject.transform.parent = transform;
                textObject.transform.localPosition = Vector3.back;

                _textMesh = textObject.AddComponent<TextMesh>();
                _textMesh.fontSize = 20;
                _textMesh.characterSize = 0.1f;
            }

            _textMesh.text = (display) ? _tile.Position.ToString() : "";
        }   
    }
}
