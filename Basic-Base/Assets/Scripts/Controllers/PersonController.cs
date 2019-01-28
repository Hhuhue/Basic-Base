using Assets.Scripts.Models;
using Assets.Scripts.Models.Entities;
using Assets.Scripts.Tools;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class PersonController : MonoBehaviour
    {
        public Living _person;
        private Game _game;

        void Start()
        {
            _game = GameProvider.Game();
            _person.Selected = false;
        }

        void Update()
        {
            if (_person != null)
            {
                _person.Tick();

                transform.position = _person.Position - (Vector3)_game.GetViewOrigin();
            }
        }

        public void SetPerson(Living person)
        {
            _person = person;
        }

        public Living GetPerson()
        {
            return _person;
        }

        public void SetPosition(Vector3 position)
        {
            _person.SetPosition(position + (Vector3)_game.GetViewOrigin());
        }

        public void Translate(Vector2 move)
        {
            transform.position += new Vector3(move.x, move.y, 0);
        }

        public void SetSelected(bool state)
        {
            _person.Selected = state;
            string iconPath = !state ? "" : Game.SpritesPath + "circle";

            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(iconPath);
        }

        public void GoToPosition(Vector2 destination)
        {
            _person.GoToPosition(destination + _game.GetViewOrigin());
        }
    }
}
