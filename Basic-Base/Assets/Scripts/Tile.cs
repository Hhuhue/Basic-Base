using UnityEngine;
using System.Collections;
using TileType = TileController.LandType;
using Orientation = TileController.Orientation;
using LandType = LandGenerator.LandPieceType;

public class Tile
{
    public TileType TileType { get; set; }

    public LandType LandType { get; set; }

    public Orientation Orientation { get; set; }

    public Vector2 Position { get; set; }
}
