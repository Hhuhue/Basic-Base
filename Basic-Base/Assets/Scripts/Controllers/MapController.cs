using System.Globalization;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Entities;
using Assets.Scripts.Models.Mapping;
using Assets.Scripts.Tools;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    /// <summary>
    /// Class initializing the map
    /// </summary>
    public class MapController : MonoBehaviour, IFactoryObserver
    {
        public bool UseMenuConfig;
        public int Width;
        public int Height;
        public int SmoothCount;
        public string Seed;
        public bool UseRandomSeed;

        [Range(0, 100)]
        public int FillPercentage = 50;

        [Range(0, 100)]
        public int ForestPercentage = 50;

        [Range(0, 100)]
        public int MountainPercentage = 5;

        [Range(0, 100)]
        public int CoastPercentage = 20;

        public GameObject EntityContainer;
        public GameObject MapButton;

        void Start()
        {
            loadMap(HomeController.GetConfig() ?? new Config());
            setCamera();
            setViewManager();
            GameProvider.Game().AddFactoryObserver(this);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //Todo: Change save mechanic
                //_map.SaveMap();
            }
        }

        /// <summary>
        /// Sets the view controller
        /// </summary>
        private void setViewManager()
        {
            ViewController controller = gameObject.AddComponent<ViewController>();
            controller.EntityContainer = EntityContainer;
            controller.MapButton = MapButton;
        }
    
        /// <summary>
        /// Initializes the game
        /// </summary>
        /// <param name="menuConfig">The game configuration. </param>
        private void loadMap(Config menuConfig)
        {
            if (!UseMenuConfig)
            {
                if (UseRandomSeed) Seed = System.DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

                menuConfig.MapHeight = Height;
                menuConfig.MapWidth = Width;
                menuConfig.FillRatio = FillPercentage;
                menuConfig.ForestRatio = ForestPercentage;
                menuConfig.MountainRatio = MountainPercentage;
                menuConfig.CoastRatio = CoastPercentage;
                menuConfig.Seed = new System.Random(Seed.GetHashCode());
                menuConfig.SmoothCount = SmoothCount;
            }

            GameProvider.Initialize(menuConfig);
        }

        /// <summary>
        /// Sets the camera positions
        /// </summary>
        private void setCamera()
        {
            Vector3 cameraPosition = new Vector3((float)View.VIEW_WIDTH / 2 + 0.5f, (float)View.VIEW_HEIGHT / 2 + 0.5f, -5);
        
            Camera.main.transform.position = cameraPosition;
        }

        public void OnEntityCreated(Entity createdEntity)
        {
            
        }

        public void OnPersonCreated(Entity createdEntity)
        {
            GameObject person = Instantiate(Resources.Load<GameObject>("Prefabs/Person"));
            person.GetComponent<PersonController>().SetPerson((Living) createdEntity);
            person.transform.position = createdEntity.Position;
            person.transform.parent = EntityContainer.transform;
        }
    }
}