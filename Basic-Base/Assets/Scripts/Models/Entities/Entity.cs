using UnityEngine;

namespace Assets.Scripts.Models.Entities
{
    public abstract class Entity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Vector3 Position { get; set; }

        public bool Selected { get; set; }
    }
}