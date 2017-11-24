using UnityEngine;

namespace Assets.Scripts.Models.Structures
{
    public class Node
    {
        public Node PreviousNode { get; set; }

        public Step Step { get; set; }

        public Vector2 Position { get; set; }

        public float Distance { get; set; }
    }
}
