using Assets.Scripts.Models.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models.Mapping
{
    public class Tile
    {
        public TileType Type { get; set; }

        public TileType Icon { get; set; }

        public Map.Orientation Orientation { get; set; }

        public Vector2 Position { get; set; }

        public TileType GetGlobalType()
        {
            return Icon == TileType.DEFAULT ? Type : Icon;
        }

        /// <summary>
        /// Gives a vector for an orientation value.
        /// </summary>
        /// <param name="orientation">The orientation value. </param>
        /// <returns>A vector for the orientation value. </returns>
        public static Vector3 OrientationToVector(Map.Orientation orientation)
        {
            switch (orientation)
            {
                case Map.Orientation.Bottom:
                case Map.Orientation.BottomLeft:
                    return new Vector3(0, 0, 90);

                case Map.Orientation.Right:
                case Map.Orientation.BottomRight:
                    return new Vector3(0, 0, 180);

                case Map.Orientation.Top:
                case Map.Orientation.TopRight:
                    return new Vector3(0, 0, -90);

                default:
                    return new Vector3(0, 0, 0);
            }
        }

        public Action[] GetActions(ActionBase actionBase)
        {
            //actionBase.Location = Position;
            List<Action> actions = new List<Action>();

            if (!actionBase.FirstPersonSpawned)
            {
                actions.Add(new SpawnAction(actionBase.EFactory, actionBase.Location, "Start here."));
            }

            foreach (Entity entity in actionBase.Selection)
            {
                actions.AddRange(entity.GetActions(actionBase));
            }

            return actions.ToArray();
        }

        public override string ToString()
        {
            return GetGlobalType() + " at " + Position + ". Orr : " + Orientation;
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
            MOUNTAIN_TOP
        }
    }
}
