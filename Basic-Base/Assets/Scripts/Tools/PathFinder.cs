using UnityEngine;
using System.Collections;

public static class PathFinder
{
    public static Vector2[] GetPath(Tile currentPosition, Tile destination, Land land)
    {


        return new Vector2[0];
    }

    private struct Node
    {
        public int Cost { get; set; }

        public Tile Position { get; set; }
    }
}
