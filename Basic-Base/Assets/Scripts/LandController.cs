﻿using UnityEngine;
using System.Linq;
using LandType = Land.LandType;

public class LandController : MonoBehaviour
{
    public GameObject map;

    private Map resources;
    private GameObject[,] childs;

    void Start()
    {
        MapController mapGenerator = map.GetComponent<MapController>();
        resources = mapGenerator.GetMap();

        childs = new GameObject[mapGenerator.width, mapGenerator.height];

        int landCount = 0;
        for (int x = 0; x < mapGenerator.width; x++)
        {
            for (int y = 0; y < mapGenerator.height; y++)
            {
                GameObject land = new GameObject("Land-" + ++landCount);
                land.transform.parent = transform;
                land.transform.position = new Vector3(x + 0.5f, y + 0.5f, 0);
                land.SetActive(false);
                
                childs[x, y] = land;
            }
        }
    }

    public void DrawLand(Tile tile)
    {
        GameObject land = childs[(int)tile.Position.x, (int)tile.Position.y];
        land.SetActive(true);

        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                Vector2 position = tile.Position;

                GameObject landPiece = Instantiate(Resources.Load<GameObject>("Prefabs/LandPiece"));
                landPiece.transform.position = new Vector3(position.x + 0.05f + x * 0.1f, position.y + 0.05f + y * 0.1f, 0);
                landPiece.transform.parent = transform;
                landPiece.transform.localScale = new Vector3(1, 1, 1);

                SpriteRenderer renderer = landPiece.GetComponent<SpriteRenderer>();
                string path = tile.LandType != LandType.WATER
                    ? LandType.GRASS.ToString().ToLower()
                    : LandType.WATER.ToString().ToLower();

                renderer.sprite = Resources.Load<Sprite>("Sprites/LandTiles/" + path);
                Debug.Log(position.x + " " + position.y + " " + tile.TileType);
                LandType landType = resources.GetLand((int)position.x, (int)position.y).GetLandPiece(x, y).LandType;

                if (landType == LandType.GRASS || landType == LandType.WATER) continue;

                GameObject icon = Instantiate(Resources.Load<GameObject>("Prefabs/TileIcon"));
                icon.transform.parent = landPiece.transform;
                icon.transform.position = landPiece.transform.position + Vector3.back;
                icon.GetComponent<IconController>().SetSprite("Sprites/" + landType.ToString().ToLower());
            }
        }
    }
}
