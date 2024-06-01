using System.Collections.Generic;
using DungeonData;
using Graph;
using Graph.Delone;
using UnityEngine;
using Utils;

namespace Generation
{
    public class GenerationContext
    {
        public static GenerationContext CurrentContext;

        public Grid3D<Cell> Grid { get; private set; }
        public List<Room> Rooms { get; private set; }
        public Delone3D DeloneGraph { get; private set; }
        public HashSet<Edge> SpanningTree { get; private set; }
        public List<List<Vector3Int>> Paths { get; } = new();
        public Dictionary<string, double> Rates { get; private set; }

        public Vector3Int GridSize => Grid.Size;

        public GenerationContext(Vector3Int gridSize)
        {
            Grid = new Grid3D<Cell>(gridSize);
            for (var i = 0; i < Grid.Length; i++)
            {
                var pos = Grid.GetPosition(i);
                Grid[pos] = new Cell(pos);
            }

            CurrentContext = this;
        }

        public void InitializeRooms()
        {
            Rooms = new List<Room>();
        }

        public void MarkRoomsCells()
        {
            foreach (var room in Rooms)
            {
                foreach (var pos in room.Bounds.allPositionsWithin)
                    Grid[pos].Type = CellType.Room;
            }
        }

        public void SetDeloneGraph(Delone3D deloneGraph)
        {
            DeloneGraph = deloneGraph;
        }

        public void SetSpanningTree(HashSet<Edge> spanningTree)
        {
            SpanningTree = spanningTree;
        }

        public void SetRates(Dictionary<string, double> rates)
        {
            Rates = rates;
        }

        public void PostCreation()
        {
            foreach (var room in Rooms)
            {
                room.PostCreation();
            }
        }

        public void PreTreeBuild()
        {
            foreach (var room in Rooms)
            {
                room.PreTreeBuild();
            }
        }

        public void PostGenValidation()
        {
            foreach (var room in Rooms)
            {
                room.PostGenValidate();
            }
        }
    }
}