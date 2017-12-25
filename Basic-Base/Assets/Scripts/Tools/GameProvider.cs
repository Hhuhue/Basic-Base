using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models;

namespace Assets.Scripts.Tools
{
    public static class GameProvider
    {
        private static Game _game;

        public static void Initialize(Config configuration)
        {
            _game = new Game(configuration);
        }

        public static void Initialize(Game game)
        {
            _game = game;
        }

        public static Game Game()
        {
            return _game;
        }
    }
}
