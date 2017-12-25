using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models.Entities;
using Assets.Scripts.Models.Mapping;
using Assets.Scripts.Models.Resources;
using Assets.Scripts.Tools;
using UnityEngine;

namespace Assets.Scripts.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Game
    {
        private Map _gameWorld;
        private List<Entity> _entities;
        private Config _configuration;
        private View _view;

        public static View.ViewMode ViewMode { get; set; }
        public const string SpritesPath = "Sprites/";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameConfiguration"></param>
        public Game(Config gameConfiguration)
        {
            PathFinder.Initialize(this);
            CornerSmoother.Initialize(this, gameConfiguration.Seed);

            _configuration = gameConfiguration;
            _gameWorld = new Map(gameConfiguration);
            _gameWorld.GenerateLands(gameConfiguration.Seed);
            _entities = new List<Entity>();
            _view = new View(_gameWorld);
        }

        #region Map_Functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile GetMapTile(int x, int y)
        {
            return _gameWorld.GetTile(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Land GetMapLand(int x, int y)
        {
            return _gameWorld.GetLand(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="landX"></param>
        /// <param name="landY"></param>
        /// <param name="tileX"></param>
        /// <param name="tileY"></param>
        /// <param name="type"></param>
        /// <param name="icon"></param>
        public void SetMapLandTile(int landX, int landY, int tileX, int tileY, Tile.TileType type, Tile.TileType icon)
        {
            Tile landPiece = _gameWorld.GetLand(landX, landY).GetLandPiece(tileX, tileY);
            landPiece.Type = type;
            landPiece.Icon = icon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetMapDimentions()
        {
            return new Dictionary<string, int>()
            {
                { "Height", _configuration.MapHeight },
                { "Width", _configuration.MapWidth }
            };
        }
        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile GetViewTile(int x, int y)
        {
            return _view.GetTile(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="origin"></param>
        public void SetViewOrigin(Vector2 origin)
        {
            _view.SetOrigin(origin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Vector2 GetViewOrigin()
        {
            return _view.Origin;
        }
    }
}
