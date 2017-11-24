using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models.Mapping;
using Assets.Scripts.Models.Structures;
using UnityEngine;

namespace Assets.Scripts.Tools
{
    public static class PathFinder
    {
        private static readonly Tile.TileType[] WalkableTiles = {Tile.TileType.GRASS, Tile.TileType.SAND};

        public static Map Land { get; set; }

        /// <summary>
        /// Generates a path from a given point to a destination 
        /// </summary>
        /// <param name="start">The starting point. </param>
        /// <param name="destination">The destination point. </param>
        /// <returns>The list of points that defines the path to the target. </returns>
        public static Vector2[] GetPath(Vector2 start, Vector2 destination)
        {
            //Get the tile at the start point
            Tile startTile = Land.GetLand((int)Math.Truncate(start.x / 10), (int)Math.Truncate(start.y / 10))
                .GetLandPiece((int)Math.Round(start.x) % 10, (int)Math.Round(start.y) % 10);

            //Get the tile at the destination point
            Tile destinationTile = Land.GetLand((int)Math.Truncate(destination.x / 10), (int)Math.Truncate(destination.y / 10))
                .GetLandPiece((int)Math.Round(destination.x) % 10, (int)Math.Round(destination.y) % 10);

            //Check if the destination is valid
            if (destinationTile == null || !WalkableTiles.Contains(destinationTile.GetGlobalType()))
            {
                Debug.Log("Cannot reach tile");
                return new[] {startTile.Position};
            }

            //Set the fisrt path step
            Step initialStep = new Step() {Cost = 0, Location = startTile};
            //Set the collection of steps
            SortedStepStack candidates = new SortedStepStack(destinationTile.Position, initialStep);
            List<Step> verifiedSteps = new List<Step>();

            //Until we run out of places to go or until we reach the destination
            while (candidates.Count != 0 && startTile.Position != destinationTile.Position)
            {
                Step currentStep = candidates.Pop();

                //Check if the step exists
                if (currentStep == null)
                {
                    Debug.Log("Cannot reach. Step is null");
                    return new[] { startTile.Position };
                }

                //Check if the destination is reached
                if (currentStep.Position == destinationTile.Position) return buildPath(currentStep, verifiedSteps);

                //Check the positions around the current step for steps closer to the destination
                for (float x = -1; x <= 1; x++)
                {
                    for (float y = -1; y <= 1; y++)
                    {
                        //Ignore the current position
                        if (x == 0 && y == 0) continue;

                        int landPositionX = (int) Math.Truncate(Math.Round(currentStep.Position.x * 10 + x) / 10);
                        int landPositionY = (int) Math.Truncate(Math.Round(currentStep.Position.y * 10 + y) / 10);
                        int landPiecePositionX = (int) Math.Round(currentStep.Position.x * 10 + x) % 10;
                        int landPiecePositionY = (int)Math.Round(currentStep.Position.y * 10 + y) % 10;
                        
                        //Create a step from the current position
                        Step nearStep = new Step()
                        {
                            Cost = currentStep.Cost + 1,
                            Location = Land.GetLand(landPositionX, landPositionY)
                                .GetLandPiece(landPiecePositionX, landPiecePositionY)
                        };

                        //Check if the location can be walked on 
                        if(nearStep.Location != null && !WalkableTiles.Contains(nearStep.Location.GetGlobalType())) continue;

                        //Check if the position has already been verified
                        bool isStepVerified = verifiedSteps.Select(stp => stp.Position.ToString()).ToList().IndexOf(nearStep.Position.ToString()) != -1;

                        //Check is the step is already a candidate or already verified
                        if (isStepVerified || candidates.Contains(nearStep)) continue;
                    
                        candidates.Push(nearStep);
                    }
                }

                verifiedSteps.Add(currentStep);

                if (verifiedSteps.Count >= 500)
                {
                    Debug.Log("Target too hard to reach.");
                    return new[] { startTile.Position };
                }
            }

            return new Vector2[0];
        }

        /// <summary>
        /// Extracts a path from all the verified steps
        /// </summary>
        /// <param name="finalStep">The steps that reached the destination. </param>
        /// <param name="verfiedSteps">The list of verified steps. </param>
        /// <returns></returns>
        private static Vector2[] buildPath(Step finalStep, List<Step> verfiedSteps)
        {
            List<Vector2> path = new List<Vector2>();
            List<Step> allSteps = verfiedSteps;

            Step currentStep = finalStep;

            path.Add(currentStep.Position);

            //Walk the way back to the start 
            while (allSteps.Count != 0)
            {
                //Only keep the steps with a lower cost than the current step
                allSteps = allSteps.Where(stp => stp.Cost < currentStep.Cost).ToList();

                //Check the steps that are around the current step
                for (float x = -1; x <= 1; x++)
                {
                    for (float y = -1; y <= 1; y++)
                    {
                        Vector2 currentPosition = currentStep.Position * 10 + new Vector2(x, y);

                        //Try to get the step at the current position
                        Step step = allSteps.FirstOrDefault(stp => stp.Position * 10 == currentPosition);

                        //Check if the step exists
                        if (step != null)
                        {
                            currentStep = step;
                            path.Add(step.Position);
                            break;
                        }
                    }
                }
            }

            return path.ToArray();
        }
    }
}


