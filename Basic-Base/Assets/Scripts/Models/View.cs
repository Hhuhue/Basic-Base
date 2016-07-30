using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Orientation = Map.Orientation;

public class View
{
    private readonly Tile[,] _viewField;
    private readonly Map _map;
    private Vector2 _origin;

    public View(Map map)
    {
        _viewField = new Tile[Config.ViewWidth, Config.ViewHeight];
        _origin = Vector2.zero;
        _map = map;

        UpdateView();
    }

    public Tile[,] GetView()
    {
        return _viewField;
    }

    public void SetOrigin(Vector2 origin)
    {
        _origin = origin;
        UpdateView();
    }

    private void UpdateView()
    {
        for (int x = 0; x < Config.ViewWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                Vector2 tilePosition = new Vector2(_origin.x + x, _origin.y + y);
                Vector2 landPiecePosition = new Vector2((_origin.x + x) * 10 % 10, (_origin.y + y) * 10 % 10);

                Tile tile = Config.ViewMode == ViewMode.LAND
                    ? _map.GetLand((int)tilePosition.x, (int)tilePosition.y)
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
