using DungeonData;
using Graph;
using UnityEngine;

namespace Generation
{
    public static class HallwaysBuilder
    {
        private const int RoomCost = 5;

        public static void PathfindHallways(GenerationContext context)
        {
            var grid = context.Grid;
            var aStar = new Pathfinder(context.GridSize);

            foreach (var edge in context.SpanningTree)
            {
                var startRoom = (edge.U as Vertex<Room>).Item;
                var endRoom = (edge.V as Vertex<Room>).Item;

                var startPosf = startRoom.Bounds.center;
                startPosf = startPosf - new Vector3(0, (startRoom.Bounds.size.y / 2), 0);
                var endPosf = endRoom.Bounds.center;
                endPosf = endPosf - new Vector3(0, (endRoom.Bounds.size.y / 2), 0);
                var startPos = new Vector3Int((int)startPosf.x, (int)startPosf.y, (int)startPosf.z);
                var endPos = new Vector3Int((int)endPosf.x, (int)endPosf.y, (int)endPosf.z);

                var path = aStar.FindPath(startPos, endPos, (a, b) =>
                {
                    var pathCost = new Pathfinder.PathCost();

                    var delta = b.Position - a.Position;

                    if (delta.y == 0)
                    {
                        pathCost.Cost = Vector3Int.Distance(b.Position, endPos);

                        switch (grid[b.Position].Type)
                        {
                            case CellType.Room:
                                pathCost.Cost += RoomCost;
                                break;
                            case CellType.None:
                                pathCost.Cost += 1;
                                break;
                        }

                        if (endRoom.Bounds.Contains(b.Position) || startRoom.Bounds.Contains(b.Position))
                            pathCost.Cost = 0;

                        pathCost.Traversable = true;
                    }
                    else
                    {
                        if (grid[a.Position].Type != CellType.None && grid[a.Position].Type != CellType.Hallway
                            || grid[b.Position].Type != CellType.None && grid[b.Position].Type != CellType.Hallway) return pathCost;

                        pathCost.Cost = 100 + Vector3Int.Distance(b.Position, endPos); //base cost + heuristic

                        if (endRoom.Bounds.Contains(a.Position) || startRoom.Bounds.Contains(a.Position))
                            pathCost.Cost = 100000;

                        var xDir = Mathf.Clamp(delta.x, -1, 1);
                        var zDir = Mathf.Clamp(delta.z, -1, 1);
                        var verticalOffset = new Vector3Int(0, delta.y, 0);
                        var horizontalOffset = new Vector3Int(xDir, 0, zDir);

                        if (!grid.InBounds(a.Position + verticalOffset)
                            || !grid.InBounds(a.Position + horizontalOffset)
                            || !grid.InBounds(a.Position + verticalOffset + horizontalOffset))
                            return pathCost;

                        if (grid[a.Position + horizontalOffset].Type != CellType.None
                            || grid[a.Position + horizontalOffset * 2].Type != CellType.None
                            || grid[a.Position + verticalOffset + horizontalOffset].Type != CellType.None
                            || grid[a.Position + verticalOffset + horizontalOffset * 2].Type != CellType.None)
                            return pathCost;

                        pathCost.Traversable = true;
                        pathCost.IsVertical = true;
                    }

                    return pathCost;
                });

                if (path != null)
                {
                    context.Paths.Add(path);
                    for (var i = 0; i < path.Count; i++)
                    {
                        var current = path[i];

                        if (grid[current].Type == CellType.None)
                            grid[current].Type = CellType.Hallway;
                    }
                }
            }
        }
    }
}