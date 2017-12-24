using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models.Entities;
using Assets.Scripts.Models.Mapping;
using Assets.Scripts.Models.Resources;

namespace Assets.Scripts.Models
{
    public class Game
    {
        private Map _gameWorld;

        private List<Entity> _entities;

        public static View.ViewMode ViewMode { get; set; }
    }
}
