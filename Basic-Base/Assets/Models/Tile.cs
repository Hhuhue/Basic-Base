using UnityEngine;
using System.Collections.Generic;
using TileType = Map.TileType;
using Orientation = Map.Orientation;
using LandType = Land.LandType;

public class Tile
{
    public TileType TileType { get; set; }

    public LandType LandType { get; set; }

    public Orientation Orientation { get; set; }

    public Vector2 Position { get; set; }

    public List<string> Icons { get; set; }
}
