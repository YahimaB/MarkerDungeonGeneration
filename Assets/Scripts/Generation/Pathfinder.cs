using System;
using System.Collections.Generic;
using DungeonData;
using Generation;
using UnityEngine;
using Utils;
using Utils.Queues;

public class Pathfinder
{
    private static readonly Vector3Int[] neighbours =
    {
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),

        // new Vector3Int(3, 1, 0),
        // new Vector3Int(-3, 1, 0),
        // new Vector3Int(0, 1, 3),
        // new Vector3Int(0, 1, -3),
        //
        // new Vector3Int(3, -1, 0),
        // new Vector3Int(-3, -1, 0),
        // new Vector3Int(0, -1, 3),
        // new Vector3Int(0, -1, -3)
    };

    private readonly Grid3D<Node> _grid;

    public Pathfinder(Vector3Int size)
    {
        _grid = new Grid3D<Node>(size);
        for (var i = 0; i < _grid.Length; i++)
        {
            var pos = _grid.GetPosition(i);
            _grid[pos] = new Node(pos);
        }
    }

    private void ResetNodes()
    {
        _grid.DoForEach(node =>
        {
            node.Previous = null;
            node.Cost = float.PositiveInfinity;
            node.PreviousSet.Clear();
        });
    }

    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int end, Func<Node, Node, PathCost> costFunction)
    {
        ResetNodes();

        SimplePriorityQueue<Node, float> queue = new SimplePriorityQueue<Node, float>();
        HashSet<Vector3Int> closed = new HashSet<Vector3Int>();

        _grid[start].Cost = 0;
        queue.Enqueue(_grid[start], 0);

        while (queue.Count > 0)
        {
            Node node = queue.Dequeue();
            closed.Add(node.Position);

            if (node.Position == end)
                return ReconstructPath(node);

            foreach (Vector3Int offset in neighbours)
            {
                var neighbourPos = node.Position + offset;
                if (!_grid.InBounds(neighbourPos) || closed.Contains(neighbourPos) || node.PreviousSet.Contains(neighbourPos))
                    continue;

                Node neighbour = _grid[neighbourPos];

                PathCost pathCost = costFunction(node, neighbour);
                if (!pathCost.Traversable)
                    continue;

                if (pathCost.IsVertical)
                {
                    var xDir = Mathf.Clamp(offset.x, -1, 1);
                    var zDir = Mathf.Clamp(offset.z, -1, 1);
                    Vector3Int verticalOffset = new Vector3Int(0, offset.y, 0);
                    Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                    if (node.PreviousSet.Contains(node.Position + horizontalOffset) ||
                        node.PreviousSet.Contains(node.Position + horizontalOffset * 2) ||
                        node.PreviousSet.Contains(node.Position + verticalOffset + horizontalOffset) ||
                        node.PreviousSet.Contains(node.Position + verticalOffset + horizontalOffset * 2))
                        continue;
                }

                var newCost = node.Cost + pathCost.Cost;

                if (newCost < neighbour.Cost)
                {
                    neighbour.Previous = node;
                    neighbour.Cost = newCost;

                    if (queue.TryGetPriority(node, out var existingPriority))
                        queue.UpdatePriority(node, newCost);
                    else
                        queue.Enqueue(neighbour, neighbour.Cost);

                    neighbour.PreviousSet.Clear();
                    neighbour.PreviousSet.UnionWith(node.PreviousSet);
                    neighbour.PreviousSet.Add(node.Position);

                    if (pathCost.IsVertical)
                    {
                        var xDir = Mathf.Clamp(offset.x, -1, 1);
                        var zDir = Mathf.Clamp(offset.z, -1, 1);
                        Vector3Int verticalOffset = new Vector3Int(0, offset.y, 0);
                        Vector3Int horizontalOffset = new Vector3Int(xDir, 0, zDir);

                        neighbour.PreviousSet.Add(node.Position + horizontalOffset);
                        neighbour.PreviousSet.Add(node.Position + horizontalOffset * 2);
                        neighbour.PreviousSet.Add(node.Position + verticalOffset + horizontalOffset);
                        neighbour.PreviousSet.Add(node.Position + verticalOffset + horizontalOffset * 2);
                    }
                }
            }
        }

        return null;
    }

    private List<Vector3Int> ReconstructPath(Node node)
    {
        var stack = new Stack<Vector3Int>();
        while (node != null)
        {
            stack.Push(node.Position);
            node = node.Previous;
        }

        var result = new List<Vector3Int>();
        while (stack.Count > 0)
            result.Add(stack.Pop());

        return result;
    }

    public class Node
    {
        public Vector3Int Position { get; private set; }
        public Node Previous { get; set; }
        public HashSet<Vector3Int> PreviousSet { get; private set; }
        public float Cost { get; set; }

        public Node(Vector3Int position)
        {
            Position = position;
            PreviousSet = new HashSet<Vector3Int>();
        }
    }

    public struct PathCost
    {
        public bool Traversable;
        public float Cost;
        public bool IsVertical;
    }
}