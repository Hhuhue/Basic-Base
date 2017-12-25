using System.Globalization;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Mapping;
using Assets.Scripts.Tools;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class MapController : MonoBehaviour
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

        private Game _game;

        void Start()
        {
            loadMap(HomeController.GetConfig() ?? new Config());
            setCamera();
            setViewManager();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //_map.SaveMap();
            }
        }

        public Map GetMap()
        {
            return new Map(new Config());
        }

        private void setViewManager()
        {
            ViewController controller = gameObject.AddComponent<ViewController>();
            controller.EntityContainer = EntityContainer;
            controller.MapButton = MapButton;
        }
    
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
            _game = GameProvider.Game();
        }

        private void setCamera()
        {
            Vector3 cameraPosition = new Vector3((float)View.VIEW_WIDTH / 2 + 0.5f, (float)View.VIEW_HEIGHT / 2 + 0.5f, -5);
        
            Camera.main.transform.position = cameraPosition;
        }
    }
}