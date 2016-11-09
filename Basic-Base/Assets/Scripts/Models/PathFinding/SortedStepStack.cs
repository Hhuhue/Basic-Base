using System.Linq;
using UnityEngine;

public class SortedStepStack
{
    private readonly Vector2 _destination;

    public int Count { get; private set; }

    private Node _firstNode;

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

        if (_firstNode == null)
        {
            _firstNode = newNode;
            Count++;
            return;
        }

        while (currentNode != null)
        {
            if (stepDistance < currentNode.Distance)
            {
                newNode.PreviousNode = currentNode;

                if (lastNode == null)
                    _firstNode = newNode;
                else
                    lastNode.PreviousNode = newNode;

                Count++;
                break;
            }

            if (currentNode.PreviousNode == null)
            {
                currentNode.PreviousNode = newNode;
                Count++;
                break;
            }

            lastNode = currentNode;
            currentNode = currentNode.PreviousNode;
        }
    }

    public Step Pop()
    {
        Step poppedStep = _firstNode.Step;
        _firstNode = _firstNode.PreviousNode;

        Count--;

        return poppedStep;
    }

    public bool Contains(Step step)
    {
        Node currentNode = _firstNode;

        while (currentNode != null)
        {
            if (currentNode.Step.Position.ToString() == step.Position.ToString()) return true;

            currentNode = currentNode.PreviousNode;
        }

        return false;
    }

    private class Node
    {
        public Node PreviousNode { get; set; }

        public Step Step { get; set; }

        public float Distance { get; set; }
    }
}