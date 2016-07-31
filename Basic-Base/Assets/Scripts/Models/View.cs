using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Orientation = Map.Orientation;

public class View
{
    public Vector2 Origin { get; private set; }

    private readonly Tile[,] _viewField;
    private readonly Map _map;

    public View(Map map)
    {
        _viewField = new Tile[Config.ViewWidth, Config.ViewHeight];
        Origin = Vector2.zero;
        _map = map;

        UpdateView();
    }

    public Tile[,] GetView()
    {
        return _viewField;
    }

    public void SetOrigin(Vector2 origin)
    {
        Origin = origin;
        UpdateView();
    }

    private void UpdateView()
    {
        for (int x = 0; x < Config.ViewWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
            {
                Vector2 tilePosition = new Vector2(Origin.x + x, Origin.y + y);
                Vector2 landPiecePosition = new Vector2((Origin.x + x) % 10, (Origin.y + y) % 10);

                Tile tile = Config.ViewMode == ViewMode.LAND
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
