﻿using System;
using System.Collections.Generic;
using Assets.Scripts.Models.Resources;
using Assets.Scripts.Tools;
using UnityEngine;

namespace Assets.Scripts.Models.Entities
{
    public class Living : Entity
    {
        private Vector2 _currentTarget;
        private Stack<Vector2> _path;
        private Inventory _inventory;

        public Living(Vector3 position)
        {
            Position = position;
            _currentTarget = position;
            _path = new Stack<Vector2>();
        }

        public void Tick()
        {
            if ((Vector2)Position != _currentTarget)
            {
                Vector3 target3 = new Vector3(_currentTarget.x, _currentTarget.y, Position.z);
                Position = Vector3.MoveTowards(Position, target3, Time.deltaTime * 2);
            }
            else if (_path.Count != 0)
            {
                _currentTarget = _path.Pop();
            }
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
        }

        public void GoToPosition(Vector2 destination)
        {
            Vector2 personPosition = new Vector2((float)Math.Truncate(Position.x), (float)Math.Truncate(Position.y));
            Vector2 destinationPosition = new Vector2((float)Math.Truncate(destination.x), (float)Math.Truncate(destination.y));

            Vector2[] path = PathFinder.GetPath(personPosition, destinationPosition);

            _path.Clear();

            foreach (Vector2 step in path)
            {
                _path.Push(step * 10 + new Vector2(0.5f, 0.5f));
            }

            _currentTarget = _path.Pop();
        }

        public override Action[] GetActions(ActionBase actionBase)
        {
            return new Action[] { new MoveAction(this, actionBase.Location, "Go there") };
        }
    }
}

