using Assets.Scripts.Models;
using Assets.Scripts.Models.Entities;
using Assets.Scripts.Tools;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PersonController : MonoBehaviour
    {
        public Living Person { get; set; }
        private Game _game;

        void Start()
        {
            _game = GameProvider.Game();
            Person = new Living((Vector2) transform.position + _game.GetViewOrigin());
            Person.Selected = false;
        }

        void Update()
        {
            Person.Tick((Vector2)transform.position + _game.GetViewOrigin());

            transform.position = Person.Position - (Vector3)_game.GetViewOrigin();
        }

        public void SetPosition(Vector3 position)
        {
            Person.SetPosition((Vector2)position + _game.GetViewOrigin());
        }

        public void Translate(Vector2 move)
        {
            transform.position += new Vector3(move.x, move.y, 0);
        }

        public void SetSelected(bool state)
        {
            Person.Selected = state;
            string iconPath = !state ? "" : Game.SpritesPath + "circle";

            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(iconPath);
        }

        public void GoToPosition(Vector2 destination)
        {
            Person.GoToPosition(destination + _game.GetViewOrigin());
        }
    }
}
