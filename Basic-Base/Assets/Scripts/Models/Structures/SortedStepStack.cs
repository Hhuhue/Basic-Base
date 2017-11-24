using UnityEngine;

namespace Assets.Scripts.Models.Structures
{
    /// <summary>
    /// A stack of steps sorted by distance from the destination
    /// </summary>
    public class SortedStepStack
    {
        private readonly Vector2 _destination;

        public int Count { get; private set; }

        private Node _firstNode;

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="destination">The destination point. </param>
        /// <param name="firstStep">The initial step. </param>
        public SortedStepStack(Vector2 destination, Step firstStep)
        {
            _destination = destination;
            _firstNode = new Node()
            {
                Distance = Vector2.Distance(firstStep.Location.Position, _destination),
                Step = firstStep,
                PreviousNode = null
            };

            Count = 1;
        }

        /// <summary>
        /// Adds a step to the stack.
        /// </summary>
        /// <param name="step">The step to add. </param>
        public void Push(Step step)
        {
            float stepDistance = Vector2.Distance(step.Location.Position, _destination);

            Node currentNode = _firstNode;
            Node lastNode = null;
            Node newNode = new Node()
            {
                Distance = stepDistance,
                Step = step,
                PreviousNode = null
            };

            //Check if the stack is empty
            if (_firstNode == null)
            {
                _firstNode = newNode;
                Count++;
                return;
            }

            //Add the step according to its distance from the destination
            while (currentNode != null)
            {
                //Check if the current step is at a greater distance from the target than the step to add
                if (stepDistance < currentNode.Distance)
                {
                    //Add the step between the last step and the current step
                    newNode.PreviousNode = currentNode;

                   if (lastNode == null)
                        _firstNode = newNode;
                    else
                        lastNode.PreviousNode = newNode;

                    Count++;
                    break;
                }

                //Check if we are at the end of the stack
                if (currentNode.PreviousNode == null)
                {
                    //Add the step at the end of the stack
                    currentNode.PreviousNode = newNode;
                    Count++;
                    break;
                }

                lastNode = currentNode;
                currentNode = currentNode.PreviousNode;
            }
        }

        /// <summary>
        /// Returns the step of the stack with the shortest distance from the destination
        /// </summary>
        /// <returns>The step of the stack with the shortest distance from the destination</returns>
        public Step Pop()
        {
            Step poppedStep = _firstNode.Step;
            _firstNode = _firstNode.PreviousNode;

            Count--;

            return poppedStep;
        }

        /// <summary>
        /// Checks if the stack contains the given step.
        /// </summary>
        /// <param name="step">The step to check. </param>
        /// <returns></returns>
        public bool Contains(Step step)
        {
            Node currentNode = _firstNode;

            while (currentNode != null)
            {
                //Check if the current step is at the same position than the given step
                if (currentNode.Step.Position.ToString() == step.Position.ToString()) return true;

                currentNode = currentNode.PreviousNode;
            }

            return false;
        }
    }
}