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

    public void MoveView(Orientation direction, float zoomSize)
    {
        Vector2 move = Vector2.zero;

        float yTranslation = Config.ViewHeight - zoomSize * 2;
        float xTranslation = yTranslation * 2;

        switch (direction)
        {
            case Orientation.TOP:
                move.y = -yTranslation;
                break;

            case Orientation.BOTTOM:
                move.y = yTranslation;
                break;

            case Orientation.LEFT:
                move.x = xTranslation;
                break;

            case Orientation.RIGHT:
                move.x = -xTranslation;
                break;
        }

        if (move.x + _origin.x + Config.ViewWidth >= Config.MapWidth)
            move.x = Config.MapWidth - _origin.x - Config.ViewWidth;

        if (move.y + _origin.y + Config.MapHeight >= Config.MapHeight)
            move.y = Config.MapHeight - _origin.y - Config.ViewHeight;

        if (move.x + _origin.x <= 0)
            move.x = -_origin.x;

        if (move.y + _origin.y <= 0)
            move.y = -_origin.y;

        _origin += move;

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
