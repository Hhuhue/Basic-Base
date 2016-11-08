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

        Tile startTile = Land.GetLand((int)start.x / 10, (int)start.y / 10)
            .GetLandPiece((int)start.x % 10, (int)start.y % 10);
        Tile destinationTile = Land.GetLand((int)destination.x / 10, (int)destination.y / 10)
            .GetLandPiece((int)destination.x % 10, (int)destination.y % 10);

        Debug.Log(startTile.ToString());
        Debug.Log(destinationTile.ToString());

        Step initialStep = new Step() {Cost = 0, Location = startTile};
        SortedSteps candidates = new SortedSteps(destinationTile.Position, initialStep);
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
                    Vector2 landPosition = new Vector2((currentStep.Position.x * 10 + x) / 10, (currentStep.Position.y * 10 + y) / 10);
                    Vector2 landPiecePosition = new Vector2((currentStep.Position.x * 10 + x) % 10, (currentStep.Position.y * 10 + y) % 10);

                    Step nearStep = new Step()
                    {
                        Cost = currentStep.Cost + 1,
                        Location = Land.GetLand((int)landPosition.x, (int)landPosition.y)
                            .GetLandPiece((int)landPiecePosition.x, (int)landPiecePosition.y)
                    };

                    if(nearStep.Location.GetGlobalType() != Tile.TileType.GRASS) continue;

                    if (x == 0 && y == 0) continue;

                    if (candidates.ContainsAtLessCost(nearStep, new Step[0]) || candidates.ContainsAtLessCost(nearStep, verifiedSteps.ToArray()))
                        continue;
                    
                    candidates.Add(nearStep);
                }
            }

            verifiedSteps.Add(currentStep);
        }

        return new Vector2[0];
    }

    private static Vector2[] BuildPath(Step finalStep, List<Step> verfiedSteps)
    {
        List<Step> path = new List<Step>();

        return verfiedSteps.Select(x => x.Position).ToArray();
    }

    private struct Step
    {
        public int Cost { get; set; }

        public Tile Location { get; set; }

        public Vector2 Position { get { return Location.Position; } }
    }

    private struct SortedSteps
    {
        private readonly Vector2 _destination;

        private float[] _distances;

        private Step[] _steps;

        public int Count { get; private set; }

        public SortedSteps(Vector2 destination, Step firstStep) : this()
        {
            _destination = destination;
            _distances = new float[Config.ViewHeight * Config.ViewWidth];
            _steps = new Step[Config.ViewHeight*Config.ViewWidth];
            
            for (int i = 0; i < _distances.Length; i++)
            {
                _distances[i] = -1;
                _steps[i].Cost = -1;
            }

            _steps[0] = firstStep;
            _distances[0] = Vector2.Distance(firstStep.Location.Position, _destination);

            Count = 1;
        }

        public void Add(Step step)
        {
            float stepDistance = Vector2.Distance(step.Location.Position, _destination);

            for (int i = 0; i < _distances.Length; i++)
            {
                if (_distances[i] <= stepDistance && _distances[i] != -1) continue;

                Step stepTemp = _steps[i];
                float distanceTemp = _distances[i];

                _distances[i] = stepDistance;
                _steps[i] = step;

                while (distanceTemp != -1 && i + 1 < _distances.Length)
                {
                    i++;
                    float distanceToShift = _distances[i];
                    Step stepToshift = _steps[i];

                    _distances[i] = distanceTemp;
                    distanceTemp = distanceToShift;

                    _steps[i] = stepTemp;
                    stepTemp = stepToshift;
                }

                break;
            }

            Count++;
        }

        public Step Pop()
        {
            Step poppedStep = _steps[0];

            for (int i = 0; i < _steps.Length; i++)
            {
                _distances[i] = _distances[i + 1];

                if (_distances[i] == -1)
                {
                    _steps[i] = new Step() {Cost = -1};
                    break;
                }
                
                _steps[i] = _steps[i + 1];
            }

            Count--;

            return poppedStep;
        }

        public bool ContainsAtLessCost(Step step, Step[] steps)
        {
            if (steps.Length == 0) steps = _steps;

            int matchingStepIndex = steps.Where(x => x.Cost < -1).Select(x => x.Position).ToList().IndexOf(step.Position);

            if (matchingStepIndex == -1) return false;

            return steps[matchingStepIndex].Cost < step.Cost;
        }
    }
}


