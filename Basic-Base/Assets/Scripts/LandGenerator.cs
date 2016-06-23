using UnityEngine;
using System.Collections;
using TileType = TileController.LandType;

public class LandGenerator : MonoBehaviour
{
    private GameObject[,] landscape;

    void Start()
    {
        landscape = new GameObject[10, 10];
    }

    void Update()
    {

    }

    public void GenerateLand(TileType type, int xPosition, int yPosition)
    {
        for(int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                GameObject landPiece = Instantiate(Resources.Load<GameObject>("Prefabs/LandPiece"));
                landPiece.transform.position = new Vector3(xPosition + 0.05f + x * 0.1f, yPosition + 0.05f + y * 0.1f, 0);
                landPiece.transform.parent = transform;
            }
        }        
    }
}