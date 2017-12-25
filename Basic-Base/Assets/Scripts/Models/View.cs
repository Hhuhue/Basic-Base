using Assets.Scripts.Models.Mapping;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class View
    {
        public const int VIEW_WIDTH = 50;
        public const int VIEW_HEIGHT = 25;

        public Vector2 Origin { get; private set; }

        private readonly Tile[,] _viewField;
        private readonly Map _map;

        public View(Map map)
        {
            _viewField = new Tile[VIEW_WIDTH, VIEW_HEIGHT];
            Origin = Vector2.zero;
            _map = map;

            updateView();
        }

        public Tile GetTile(int x, int y)
        {
            //Todo: Secure
            return _viewField[x,y];
        }

        public void SetOrigin(Vector2 origin)
        {
            Origin = origin;
            updateView();
        }

        private void updateView()
        {
            for (int x = 0; x < VIEW_WIDTH; x++)
            {
                for (int y = 0; y < VIEW_HEIGHT; y++)
                {
                    Vector2 tilePosition = new Vector2(Origin.x + x, Origin.y + y);
                    Vector2 landPiecePosition = new Vector2((Origin.x + x) % 10, (Origin.y + y) % 10);

                    Tile tile = Game.ViewMode == ViewMode.LAND
                        ? _map.GetLand((int)tilePosition.x / 10, (int)tilePosition.y / 10)
                            .GetLandPiece((int)landPiecePosition.x, (int)landPiecePosition.y)
                        : _map.GetTile((int)tilePosition.x, (int)tilePosition.y);

                    _viewField[x, y] = tile;
                }
            }
        }

        public enum ViewMode
        {
            MAP,
            LAND
        }
    }
}
