using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Cryptography.X509Certificates;

public static class PathFinder
{
    public static Map Land { get; set; }

    public static Vector2[] GetPath(Vector2 start, Vector2 destination)
    {
        Tile startTile = Land.GetLand((int)Math.Truncate(start.x / 10), (int)Math.Truncate(start.y / 10))
            .GetLandPiece((int)Math.Round(start.x % 10), (int)Math.Round(start.y % 10));
        Tile destinationTile = Land.GetLand((int)Math.Truncate(destination.x / 10), (int)Math.Truncate(destination.y / 10))
            .GetLandPiece((int)Math.Round(destination.x % 10), (int)Math.Round(destination.y % 10));

        if (destinationTile == null)
        {
            Debug.Log("Cannot reach");
            return new[] {start};
        }

        Step initialStep = new Step() {Cost = 0, Location = startTile};
        SortedStepStack candidates = new SortedStepStack(destinationTile.Position, initialStep);
        List<Step> verifiedSteps = new List<Step>();

        while (candidates.Count != 0 && startTile.Position != destinationTile.Position)
        {
            Step currentStep = candidates.Pop();

            if (currentStep.Cost == -1) return new Vector2[0];

            if (currentStep.Position == destinationTile.Position) return BuildPath(currentStep, verifiedSteps);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2 landPosition = new Vector2(currentStep.Position.x + x / 10, currentStep.Position.y + y / 10);
                    Vector2 landPiecePosition = new Vector2((currentStep.Position.x * 10 + x) % 10, (currentStep.Position.y * 10 + y) % 10);
                    
                    Step nearStep = new Step()
                    {
                        Cost = currentStep.Cost + 1,
                        Location = Land.GetLand((int)Math.Truncate(landPosition.x), (int)Math.Truncate(landPosition.y))
                            .GetLandPiece((int)Math.Round(landPiecePosition.x), (int)Math.Round(landPiecePosition.y))
                    };

                    if(nearStep.Location != null && nearStep.Location.GetGlobalType() != Tile.TileType.GRASS) continue;

                    if (x == 0 && y == 0) continue;

                    bool isStepVerified = verifiedSteps.Select(stp => stp.Position.ToString()).ToList().IndexOf(nearStep.Position.ToString()) != -1;

                    if (candidates.Contains(nearStep) || isStepVerified)
                        continue;
                    
                    candidates.Push(nearStep);
                }
            }

            verifiedSteps.Add(currentStep);
        }

        return new Vector2[0];
    }

    private static Vector2[] BuildPath(Step finalStep, List<Step> verfiedSteps)
    {
        List<Step> path = new List<Step>();

        verfiedSteps.Add(finalStep);

        return verfiedSteps.OrderByDescending(x => x.Cost).Select(x => x.Position).ToArray();
    }
}


