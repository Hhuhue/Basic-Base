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

        private Map _map;

        void Start()
        {
            loadMap();
            setCamera();
            setViewManager();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _map.SaveMap();
            }
        }

        public Map GetMap()
        {
            return _map;
        }

        void setViewManager()
        {
            ViewController controller = gameObject.AddComponent<ViewController>();
            controller.ViewField = new View(_map);
            controller.EntityContainer = EntityContainer;
            controller.MapButton = MapButton;
        
            PersonController.ViewField = controller.ViewField;
        }
    
        private void loadMap()
        {
            if (!UseMenuConfig)
            {
                if (UseRandomSeed) Seed = System.DateTime.UtcNow.ToString();

                Config.MapHeight = Height;
                Config.MapWidth = Width;
                Config.FillRatio = FillPercentage;
                Config.ForestRatio = ForestPercentage;
                Config.MountainRatio = MountainPercentage;
                Config.CoastRatio = CoastPercentage;
                Config.Seed = new System.Random(Seed.GetHashCode());
                Config.SmoothCount = SmoothCount;
                Config.SpritesPath = "Sprites/";
            }

            _map = new Map();
            PathFinder.Land = _map;
        }

        private void setCamera()
        {
            Vector3 cameraPosition = new Vector3((float)Config.ViewWidth / 2 + 0.5f, (float)Config.ViewHeight / 2 + 0.5f, -5);
        
            Camera.main.transform.position = cameraPosition;
        }
    }
}