﻿using UnityEngine;
using System.Collections.Generic;
using Orientation = Map.Orientation;

public class Tile
{
    public TileType Type { get; set; }

    public TileType Icon { get; set; }

    public Orientation Orientation { get; set; }

    public Vector2 Position { get; set; }

    public TileType GetGlobalType()
    {
        return Icon == TileType.DEFAULT ? Type : Icon;
    }

    public enum TileType
    {
        DEFAULT,
        WATER,
        PLAIN,
        FOREST,
        MOUNTAIN,
        COAST,
        COAST_CORNER,
        COAST_END,
        TREE,
        PINE,
        ROCK,
        SAND,
        GRASS,
        CLIFF,
        CLIFF_CORNER,
        MOUNTAIN_FACE,
        MOUNTAIN_CORNER,
        MOUNTAIN_TOP,
    }
}
