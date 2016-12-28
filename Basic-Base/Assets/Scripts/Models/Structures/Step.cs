using Assets.Scripts.Models.Mapping;
using UnityEngine;

namespace Assets.Scripts.Models.PathFinding
{
    public class Step
    {
        public int Cost { get; set; }

        public Tile Location { get; set; }

        public Vector2 Position
        {
            get { return Location.Position; }
        }

        public override string ToString()
        {
            return "Cost : " + Cost + " at " + Position;
        }
    }
}
