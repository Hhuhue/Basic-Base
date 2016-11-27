using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Models.PathFinding
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
