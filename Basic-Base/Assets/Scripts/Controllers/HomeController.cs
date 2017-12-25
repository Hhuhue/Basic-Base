using System;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Mapping;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    /// <summary>
    /// Class that manages the game menu
    /// </summary>
    public class HomeController : MonoBehaviour
    {
        public GameObject HomeGroup;
        public GameObject OptionsGroup;
        public GameObject SeedInput;
        public GameObject UseRandomInput;
        public GameObject WidthInput;
        public GameObject HeightInput;
        public GameObject LandInput;
        public GameObject SelectedLand;
        public GameObject ForestInput;
        public GameObject SelectedForest;
        public GameObject MountainInput;
        public GameObject SelectedMountain;
        public GameObject CoastInput;
        public GameObject SelectedCoast;

        private string _previousSeed;
        private Map _map;
        private static Config _gameConfig;

        void Start()
        {
            _gameConfig = new Config();
            loadMap(_gameConfig);

            //Generates a map for the background of the menu
            for (int x = 0; x < _gameConfig.MapWidth; x++)
            {
                for (int y = 0; y < _gameConfig.MapHeight; y++)
                {
                    Tile tile = _map.GetTile(x, y);

                    GameObject tileObject = Instantiate(Resources.Load<GameObject>("Prefabs/Tile"));
                    tileObject.transform.parent = transform;
                    tileObject.transform.position = new Vector3(x + 0.5f, y + 0.5f, 3);
                    tileObject.transform.localScale = Vector3.one;

                    TileController controller = tileObject.GetComponent<TileController>();
                    controller.SetTile(tile);
                    controller.SetPosition(x, y);
                    controller.enabled = false;
                }
            }

            _previousSeed = DateTime.UtcNow.ToString();

            //Set the camera
            Camera.main.transform.position = new Vector3(25, 12.5f, -3);
            Camera.main.orthographicSize = 9;

            _gameConfig.SmoothCount = 1;
            _gameConfig.Seed = new System.Random(_previousSeed.GetHashCode());
        }

        /// <summary>
        /// Gets the game configuration edited by the user. 
        /// </summary>
        /// <returns>The game configuration edited by the user.</returns>
        public static Config GetConfig()
        {
            return _gameConfig;
        }

        /// <summary>
        /// Loads the game scene
        /// </summary>
        public void StartGame()
        {
            SceneManager.LoadScene("main");
        }

        /// <summary>
        /// Renders the options menu
        /// </summary>
        public void DisplayOptions()
        {
            HomeGroup.SetActive(false);
            OptionsGroup.SetActive(true);
        }

        /// <summary>
        /// Reverts the configuration values entered by the user.
        /// </summary>
        /// <param name="gameConfig">The game configuration. </param>
        public void CancelOption(Config gameConfig)
        {
            HeightInput.GetComponent<InputField>().text = gameConfig.MapHeight.ToString();
            WidthInput.GetComponent<InputField>().text = gameConfig.MapWidth.ToString();
            LandInput.GetComponent<Slider>().value = (float)(gameConfig.FillRatio - 40) / 5;
            ForestInput.GetComponent<Slider>().value = (float)(gameConfig.ForestRatio - 10) / 15;
            MountainInput.GetComponent<Slider>().value = (float)(gameConfig.MountainRatio - 8) / 6;
            CoastInput.GetComponent<Slider>().value = (float)(gameConfig.CoastRatio - 15) / 10;
            SeedInput.GetComponent<Text>().text = _previousSeed;

            HomeGroup.SetActive(true);
            OptionsGroup.SetActive(false);
        }

        /// <summary>
        /// Saves the configuration values entered by the user.
        /// </summary>
        /// <param name="gameConfig">The game configuration. </param>
        public void SaveOption(Config gameConfig)
        {
            string seed = UseRandomInput.GetComponent<Toggle>().isOn 
                ? DateTime.UtcNow.ToString()
                : SeedInput.GetComponent<Text>().text;

            gameConfig.MapHeight = int.Parse(HeightInput.GetComponent<InputField>().text);
            gameConfig.MapWidth = int.Parse(WidthInput.GetComponent<InputField>().text);
            gameConfig.FillRatio = 40 + int.Parse(LandInput.GetComponent<Slider>().value.ToString()) * 5;
            gameConfig.ForestRatio = 10 + int.Parse(ForestInput.GetComponent<Slider>().value.ToString()) * 15;
            gameConfig.MountainRatio = 8 + int.Parse(MountainInput.GetComponent<Slider>().value.ToString()) * 6;
            gameConfig.CoastRatio = 15 + int.Parse(CoastInput.GetComponent<Slider>().value.ToString()) * 10;
            gameConfig.Seed = new System.Random(seed.GetHashCode());

            HomeGroup.SetActive(true);
            OptionsGroup.SetActive(false);
        }

        /// <summary>
        /// Updates the forest parameter text.
        /// </summary>
        public void UpdateForestText()
        {
            updateSelectionText(ForestInput, SelectedForest);
        }

        /// <summary>
        /// Updates the mountain parameter text.
        /// </summary>
        public void UpdateMountainText()
        {
            updateSelectionText(MountainInput, SelectedMountain);
        }

        /// <summary>
        /// Updates the coast parameter text.
        /// </summary>
        public void UpdateCoastText()
        {
            updateSelectionText(CoastInput, SelectedCoast);
        }

        /// <summary>
        /// Updates the land parameter text.
        /// </summary>
        public void UpdateLandText()
        {
            int value = (int)LandInput.GetComponent<Slider>().value;
            string text = "ERROR";

            switch (value)
            {
                case 0:
                    text = "Rare land";
                    break;
                case 1:
                    text = "Island";
                    break;
                case 2:
                    text = "Big island";
                    break;
                case 3:
                    text = "Continent";
                    break;
                case 4:
                    text = "Big continent";
                    break;
            }

            SelectedLand.GetComponent<Text>().text = text;
        }

        /// <summary>
        /// Sets the text of an input given its value
        /// </summary>
        /// <param name="slider">The input. </param>
        /// <param name="selection">The input text. </param>
        private void updateSelectionText(GameObject slider, GameObject selection)
        {
            int value = (int)slider.GetComponent<Slider>().value;
            string text = "ERROR";

            switch (value)
            {
                case 0:
                    text = "Rare";
                    break;
                case 1:
                    text = "Very scarce";
                    break;
                case 2:
                    text = "Scarce";
                    break;
                case 3:
                    text = "Common";
                    break;
                case 4:
                    text = "Very common";
                    break;
            }

            selection.GetComponent<Text>().text = text;
        }

        /// <summary>
        /// Creates the background map.
        /// </summary>
        /// <param name="gameConfig">The game configuration. </param>
        private void loadMap(Config gameConfig)
        {
            gameConfig.MapHeight = View.VIEW_HEIGHT;
            gameConfig.MapWidth = View.VIEW_WIDTH;
            gameConfig.FillRatio = 55;
            gameConfig.ForestRatio = 60;
            gameConfig.MountainRatio = 20;
            gameConfig.CoastRatio = 35;
            gameConfig.Seed = new System.Random("Home".GetHashCode());
            gameConfig.SmoothCount = 1;

            _map = new Map(gameConfig);

            //Remove the background parameters
            CancelOption(gameConfig);
        }
    }
}
