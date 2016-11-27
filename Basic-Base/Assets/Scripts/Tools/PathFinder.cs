using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Models.PathFinding;

public static class PathFinder
{
    public static Map Land { get; set; }

    public static Vector2[] GetPath(Vector2 start, Vector2 destination)
    {
        Tile startTile = Land.GetLand((int)Math.Truncate(start.x / 10), (int)Math.Truncate(start.y / 10))
            .GetLandPiece((int)Math.Round(start.x) % 10, (int)Math.Round(start.y) % 10);

        Tile destinationTile = Land.GetLand((int)Math.Truncate(destination.x / 10), (int)Math.Truncate(destination.y / 10))
            .GetLandPiece((int)Math.Round(destination.x) % 10, (int)Math.Round(destination.y) % 10);

        if (destinationTile == null || destinationTile.GetGlobalType() != Tile.TileType.GRASS)
        {
            Debug.Log("Cannot reach. Tile not grass");
            return new[] {startTile.Position};
        }

        Step initialStep = new Step() {Cost = 0, Location = startTile};
        SortedStepStack candidates = new SortedStepStack(destinationTile.Position, initialStep);
        List<Step> verifiedSteps = new List<Step>();

        while (candidates.Count != 0 && startTile.Position != destinationTile.Position)
        {
            Step currentStep = candidates.Pop();

            if (currentStep == null)
            {
                Debug.Log("Cannot reach. Step is null");
                return new[] { startTile.Position };
            }

            if (currentStep.Position == destinationTile.Position) return BuildPath(currentStep, verifiedSteps);

            for (float x = -1; x <= 1; x++)
            {
                for (float y = -1; y <= 1; y++)
                {
                    int landPositionX = (int) Math.Truncate(Math.Round(currentStep.Position.x * 10 + x) / 10);
                    int landPositionY = (int) Math.Truncate(Math.Round(currentStep.Position.y * 10 + y) / 10);
                    int landPiecePositionX = (int) Math.Round(currentStep.Position.x * 10 + x) % 10;
                    int landPiecePositionY = (int)Math.Round(currentStep.Position.y * 10 + y) % 10;
                    
                    Step nearStep = new Step()
                    {
                        Cost = currentStep.Cost + 1,
                        Location = Land.GetLand(landPositionX, landPositionY)
                            .GetLandPiece(landPiecePositionX, landPiecePositionY)
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
        List<Vector2> path = new List<Vector2>();
        List<Step> allSteps = verfiedSteps;

        Step currentStep = finalStep;

        path.Add(currentStep.Position);

        while (allSteps.Count != 0)
        {
            allSteps = allSteps.Where(stp => stp.Cost < currentStep.Cost).ToList();

            for (float x = -1; x <= 1; x++)
            {
                for (float y = -1; y <= 1; y++)
                {
                    Vector2 nextPosition = currentStep.Position * 10 + new Vector2(x, y);

                    Step step = allSteps.FirstOrDefault(stp => stp.Position * 10 == nextPosition);

                    if (step == null) continue;

                    currentStep = step;
                    path.Add(step.Position);
                    break;
                }
            }
        }

        return path.ToArray();
    }
}


