using System;
using System.Collections.Generic;
using Assets.Scripts.Models.Mapping;
using Assets.Scripts.Models.Observers;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class View
    {
        public const int VIEW_WIDTH = 50;
        public const int VIEW_HEIGHT = 25;

        private Vector2 _origin;
        private readonly Tile[,] _viewField;
        private readonly Map _map;
        private IViewObserver _observer;
        private ViewMode _viewMode;

        public View(Map map)
        {
            _viewField = new Tile[VIEW_WIDTH, VIEW_HEIGHT];
            _origin = Vector2.zero;
            _map = map;
            _viewMode = ViewMode.MAP;

            updateView();
        }

        public void SetViewObserver(IViewObserver observer)
        {
            _observer = observer;
            updateView();
        }

        public void SetViewMode(ViewMode viewMode)
        {
            _viewMode = viewMode;
            _observer.ToggleTileSelector(viewMode == ViewMode.MAP);
        }

        public ViewMode GetViewMode()
        {
            return _viewMode;
        }

        public Tile GetTile(int x, int y)
        {
            if (0 <= x && x < VIEW_WIDTH && y >= 0 && y < VIEW_HEIGHT)
            {
                return _viewField[x, y];
            }

            return null;
        }

        public Vector2 GetOrigin()
        {
            return _origin;
        }

        public void SetOrigin(Vector2 origin)
        {
            bool bottomLeftIsValid = _map.IsPositionValid((int) origin.x, (int) origin.y);
            bool topRightIsValid = _map.IsPositionValid((int)origin.x + VIEW_WIDTH - 1, (int)origin.y + VIEW_HEIGHT - 1);

            bool mapViewIsValid = bottomLeftIsValid && topRightIsValid;

            bottomLeftIsValid = _map.IsPositionValid((int)(origin.x/10) , (int)(origin.y/10));
            topRightIsValid = _map.IsPositionValid((int)((origin.x+VIEW_WIDTH - 1) /10), (int)((origin.y + VIEW_HEIGHT - 1) /10));

            bool landViewIsValid = bottomLeftIsValid && topRightIsValid;

            if (mapViewIsValid || landViewIsValid)
            {
                _origin = origin;
                updateView();
            }
            else
            {
                throw new ArgumentException("Invalid view origin : " + origin);
            }
        }

        private void updateView()
        {
            for (int x = 0; x < VIEW_WIDTH; x++)
            {
                for (int y = 0; y < VIEW_HEIGHT; y++)
                {
                    Vector2 tilePosition = new Vector2(_origin.x + x, _origin.y + y);
                    Vector2 landPiecePosition = new Vector2((_origin.x + x) % 10, (_origin.y + y) % 10);

                    Tile tile = _viewMode == ViewMode.LAND
                        ? _map.GetLand((int)tilePosition.x / 10, (int)tilePosition.y / 10)
                            .GetLandPiece((int)landPiecePosition.x, (int)landPiecePosition.y)
                        : _map.GetTile((int)tilePosition.x, (int)tilePosition.y);

                    _viewField[x, y] = tile;

                    if (_observer != null)
                    {
                        _observer.TileChanged(x, y);
                    }
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
