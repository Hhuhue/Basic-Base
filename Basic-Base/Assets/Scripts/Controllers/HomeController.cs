using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Mapping;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    void Start()
    {
        LoadMap();

        for (int x = 0; x < Config.MapWidth; x++)
        {
            for (int y = 0; y < Config.ViewHeight; y++)
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

        Camera.main.transform.position = new Vector3(25, 12.5f, -3);
        Camera.main.orthographicSize = 9;
        Config.SmoothCount = 1;
        Config.SpritesPath = "Sprites/";
        Config.Seed = new System.Random(_previousSeed.GetHashCode());
    }

    public void StartGame()
    {
        SceneManager.LoadScene("main");
    }

    public void DisplayOptions()
    {
        HomeGroup.SetActive(false);
        OptionsGroup.SetActive(true);
    }

    public void CancelOption()
    {
        HeightInput.GetComponent<InputField>().text = Config.MapHeight.ToString();
        WidthInput.GetComponent<InputField>().text = Config.MapWidth.ToString();
        LandInput.GetComponent<Slider>().value = (float)(Config.FillRatio - 40) / 5;
        ForestInput.GetComponent<Slider>().value = (float)(Config.ForestRatio - 10) / 15;
        MountainInput.GetComponent<Slider>().value = (float)(Config.MountainRatio - 8) / 6;
        CoastInput.GetComponent<Slider>().value = (float)(Config.CoastRatio - 15) / 10;
        SeedInput.GetComponent<Text>().text = _previousSeed;

        HomeGroup.SetActive(true);
        OptionsGroup.SetActive(false);
    }

    public void SaveOption()
    {
        string seed = UseRandomInput.GetComponent<Toggle>().isOn 
            ? DateTime.UtcNow.ToString()
            : SeedInput.GetComponent<Text>().text;

        Config.MapHeight = int.Parse(HeightInput.GetComponent<InputField>().text);
        Config.MapWidth = int.Parse(WidthInput.GetComponent<InputField>().text);
        Config.FillRatio = 40 + int.Parse(LandInput.GetComponent<Slider>().value.ToString()) * 5;
        Config.ForestRatio = 10 + int.Parse(ForestInput.GetComponent<Slider>().value.ToString()) * 15;
        Config.MountainRatio = 8 + int.Parse(MountainInput.GetComponent<Slider>().value.ToString()) * 6;
        Config.CoastRatio = 15 + int.Parse(CoastInput.GetComponent<Slider>().value.ToString()) * 10;
        Config.Seed = new System.Random(seed.GetHashCode());

        HomeGroup.SetActive(true);
        OptionsGroup.SetActive(false);
    }

    public void UpdateForestText()
    {
        UpdateSelectionText(ForestInput, SelectedForest);
    }

    public void UpdateMountainText()
    {
        UpdateSelectionText(MountainInput, SelectedMountain);
    }

    public void UpdateCoastText()
    {
        UpdateSelectionText(CoastInput, SelectedCoast);
    }

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

    private void UpdateSelectionText(GameObject slider, GameObject selection)
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

    private void LoadMap()
    {
        Config.MapHeight = Config.ViewHeight;
        Config.MapWidth = Config.ViewWidth;
        Config.FillRatio = 55;
        Config.ForestRatio = 60;
        Config.MountainRatio = 20;
        Config.CoastRatio = 35;
        Config.Seed = new System.Random("Home".GetHashCode());
        Config.SmoothCount = 1;
        Config.SpritesPath = "Sprites/";

        _map = new Map();

        CancelOption();
    }
}
