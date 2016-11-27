using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Models.PathFinding
{
    public class Node
    {
        public Node PreviousNode { get; set; }

        public Step Step { get; set; }

        public Vector2 Position { get; set; }

        public float Distance { get; set; }
    }
}
