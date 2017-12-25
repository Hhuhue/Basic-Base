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
    /// Class managing all game data
    /// </summary>
    public class Game
    {
        public static View.ViewMode ViewMode { get; set; }
        public const string SpritesPath = "Sprites/";

        private Map _gameWorld;
        private List<Entity> _entities;
        private Config _configuration;
        private View _view;

        /// <summary>
        /// Creates an new game instance.
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
        
        /// <summary>
        /// Gets the tile of the map at the given coordinates. 
        /// </summary>
        /// <param name="x">The x position. </param>
        /// <param name="y">The y position. </param>
        /// <returns>The tile at the given coordinates. </returns>
        public Tile GetMapTile(int x, int y)
        {
            return _gameWorld.GetTile(x, y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The x position. </param>
        /// <param name="y">The y position. </param>
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
            if (ViewMode == View.ViewMode.LAND)
            {
                if (origin.x + View.VIEW_WIDTH > _configuration.MapWidth * 10)
                {
                    origin.x = _configuration.MapWidth * 10 - View.VIEW_WIDTH;
                }

                if (origin.y + View.VIEW_HEIGHT > _configuration.MapHeight * 10)
                {
                    origin.y = _configuration.MapHeight * 10 - View.VIEW_HEIGHT;
                }
            }

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
