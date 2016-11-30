using UnityEngine;

namespace Assets.Scripts.Models.Entities
{
    public class Entity
    {
        public string Name { get; set; }

        public Vector3 Position { get; set; }

        public bool Selected { get; set; }
    }
}