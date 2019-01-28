using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Models.Entities;
using Assets.Scripts.Models.Mapping;
using Assets.Scripts.Models.Observers;
using Assets.Scripts.Models.Resources;
using Assets.Scripts.Tools;
using UnityEngine;

namespace Assets.Scripts.Models
{
    /// <summary>
    /// Class managing all game data
    /// </summary>
    public class Game : IFactoryObserver
    {
        public const string SpritesPath = "Sprites/";

        private Map _gameWorld;
        private List<Entity> _entities;
        private List<Entity> _selection;
        private Config _configuration;
        private View _view;
        private bool _firstPersonSpawned;
        private EntityFactory _entityFactory;

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
            _selection = new List<Entity>();
            _firstPersonSpawned = false;
            _entityFactory = new EntityFactory();
            _entityFactory.AddObserver(this);
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
        /// Gets the land of the tile at the given coordinates
        /// </summary>
        /// <param name="x">The x position. </param>
        /// <param name="y">The y position. </param>
        /// <returns></returns>
        public Land GetMapLand(int x, int y)
        {
            return _gameWorld.GetLand(x, y);
        }

        /// <summary>
        /// Sets the informations of a land piece.
        /// </summary>
        /// <param name="landX">The x position of the map tile. </param>
        /// <param name="landY">The y position of the map tile. </param>
        /// <param name="tileX">The x position of the land tile. </param>
        /// <param name="tileY">The y position of the land tile. </param>
        /// <param name="type">The new type of the tile. </param>
        /// <param name="icon">The new Icon of the tile. </param>
        public void SetMapLandTile(int landX, int landY, int tileX, int tileY, Tile.TileType type, Tile.TileType icon)
        {
            Tile landPiece = _gameWorld.GetLand(landX, landY).GetLandPiece(tileX, tileY);
            landPiece.Type = type;
            landPiece.Icon = icon;
        }

        /// <summary>
        /// Gets the map dimentions.
        /// </summary>
        /// <returns>The map dimentions. </returns>
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
        /// <param name="centerTile"></param>
        public void ChangeViewMode(View.ViewMode mode, Tile centerTile = null)
        {
            _view.SetViewMode(mode);

            if (mode == View.ViewMode.LAND && centerTile != null)
            {
                Vector2 viewCenter = new Vector2((float)Math.Truncate((float)View.VIEW_WIDTH / 2), (float)Math.Truncate((float)View.VIEW_HEIGHT / 2));
                Vector2 newOrigin = new Vector2((int)centerTile.Position.x, (int)centerTile.Position.y) * 10 - viewCenter;
                SetViewOrigin(newOrigin);
            }
            else if (mode == View.ViewMode.MAP)
            {
                int maxViewXPosition = _configuration.MapWidth - View.VIEW_WIDTH;
                int maxViewYPosition = _configuration.MapHeight - View.VIEW_HEIGHT;
                
                Vector2 viewCenter = new Vector2(View.VIEW_WIDTH / 2, View.VIEW_HEIGHT / 2);

                Vector2 focusedLandPiecePosition = GetViewOrigin() + viewCenter;
                Vector2 scaledPosition = focusedLandPiecePosition / 10;
                Vector2 scaledOrigin = scaledPosition - viewCenter;

                scaledOrigin.x = (scaledOrigin.x > maxViewXPosition) ? maxViewXPosition : (scaledOrigin.x < 0) ? 0 : scaledOrigin.x;
                scaledOrigin.y = (scaledOrigin.y > maxViewYPosition) ? maxViewYPosition : (scaledOrigin.y < 0) ? 0 : scaledOrigin.y;

                Debug.Log("Focus: " + focusedLandPiecePosition.ToString() + ", Scaled focus: " + scaledPosition.ToString() + ", Scaled origin: " + scaledOrigin.ToString());

                SetViewOrigin(scaledOrigin);
            }
        }

        public void SetViewObserver(IViewObserver observer)
        {
            _view.SetViewObserver(observer);
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
            if (_view.GetViewMode() == View.ViewMode.LAND)
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

        public void AddFactoryObserver(IFactoryObserver observer)
        {
            _entityFactory.AddObserver(observer);
        }

        /// <summary>
        /// Gets the view origin.
        /// </summary>
        /// <returns>The view's origin.</returns>
        public Vector2 GetViewOrigin()
        {
            return _view.GetOrigin();
        }

        /// <summary>
        /// Gets the current view mode
        /// </summary>
        /// <returns>The current view mode. </returns>
        public View.ViewMode GetViewMode()
        {
            return _view.GetViewMode();
        }

        /// <summary>
        /// Sets the current entity selection.
        /// </summary>
        /// <param name="entities">The selected entities.</param>
        /// <param name="append">Append the new selction to the current one or not.</param>
        public void SetSelected(List<Entity> entities, bool append)
        {
            if (append)
            {
                _selection.AddRange(entities);
            }
            else
            {
                _selection = entities;
            }
        }

        public ActionBase GetActionBase()
        {
            return new ActionBase
            {
                FirstPersonSpawned = _firstPersonSpawned,
                Selection = _selection,
                EFactory = _entityFactory
            };
        }

        public void OnEntityCreated(Entity createdEntity)
        {
            _entities.Add(createdEntity);
        }

        public void OnPersonCreated(Entity createdEntity)
        {
            if (!_firstPersonSpawned)
            {
                _firstPersonSpawned = true;
            }
        }
    }
}
