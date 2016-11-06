using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Cryptography.X509Certificates;

public static class PathFinder
{
    public static Map Land { get; set; }

    public static Vector2[] GetPath(Tile start, Tile destination)
    {
        Step initialStep = new Step {Cost = 0, Location = start};
        SortedSteps candidates = new SortedSteps(destination.Position, initialStep);
        List<Step> verifiedSteps = new List<Step>();

        while (candidates.Count != 0 && start.Position != destination.Position)
        {
            Step currentStep = candidates.Pop();

            if (currentStep.Cost == -1) return new Vector2[0];

            if (currentStep.Position == destination.Position) return BuildPath(currentStep, verifiedSteps);

            for (int x = -1; x < 1; x++)
            {
                for (int y = -1; y < 1; y++)
                {
                    Step nearStep = new Step()
                    {
                        Cost = currentStep.Cost + 1,
                        //Location = Land.GetLandPiece((int)currentStep.Position.x + x, (int)currentStep.Position.y + y)
                    };

                    if(nearStep.Location.GetGlobalType() != Tile.TileType.GRASS) continue;

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

        return path.Select(x => x.Position).ToArray();
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

            _steps[0] = firstStep;
            _distances[0] = Vector2.Distance(firstStep.Location.Position, _destination);

            for (int i = 0; i < _distances.Length; i++)
            {
                _distances[i] = -1;
            }

            Count = 1;
        }

        public void Add(Step step)
        {
            float stepDistance = Vector2.Distance(step.Location.Position, _destination);

            for (int i = 0; i < _distances.Length; i++)
            {
                if (_distances[i] <= stepDistance || _distances[i] != -1) continue;

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
                    _steps[i + 1] = new Step();
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

            int matchingStepIndex = steps.Select(x => x.Position).ToList().IndexOf(step.Position);

            if (matchingStepIndex == -1) return false;

            return steps[matchingStepIndex].Cost < step.Cost;
        }
    }
}


