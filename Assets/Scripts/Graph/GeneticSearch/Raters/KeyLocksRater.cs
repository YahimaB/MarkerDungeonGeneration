using System.Collections.Generic;
using System.Linq;
using DungeonData;
using Generation;
using UnityEngine;

namespace Graph.GeneticSearch.Raters
{
    public class KeyLocksRater : GeneticRater
    {
        public override GeneticRaterMode Mode => GeneticRaterMode.Multiply;
        public override float Weight => 1f;

        private List<Vertex<Room>> _vertices;

        private readonly Dictionary<Vertex<Room>, HashSet<Vertex<Room>>> _graph = new();

        private readonly HashSet<Vertex> _visited = new();

        public override double Rate(GenerationContext context, bool[] sln)
        {
            _graph.Clear();
            _visited.Clear();
            _keys.Clear();
            _locks.Clear();

            return CheckKeyLocks(context, sln);
        }

        public double CheckKeyLocks(GenerationContext context, bool[] sln)
        {
            var edges = context.DeloneGraph.Edges;
            _vertices = context.DeloneGraph.Vertices.Select(x => x as Vertex<Room>).ToList();
            foreach (var vertex in _vertices)
            {
                _graph[vertex] = new HashSet<Vertex<Room>>();
            }

            for (var i = 0; i < edges.Count; i++)
            {
                if (sln[i])
                {
                    var edge = edges[i];
                    _graph[edge.V as Vertex<Room>].Add(edge.U as Vertex<Room>);
                    _graph[edge.U as Vertex<Room>].Add(edge.V as Vertex<Room>);
                }
            }

            var enterVertex = _vertices.Find(x => x.Item is EnterRoom);

            Dfs(enterVertex);

            var allVisited = _visited.Count == _vertices.Count;
            return allVisited ? 0 : FailureValue;
        }

        private readonly HashSet<string> _keys = new();
        private readonly List<Vertex<Room>> _locks = new();

        private void Dfs(Vertex<Room> v)
        {
            _visited.Add(v);
            foreach (var v2 in _graph[v])
            {
                if (_visited.Contains(v2))
                {
                    continue;
                }

                var id = v2.Item.Id;

                if (v2.Item is KeyRoom)
                {

                    _keys.Add(id);
                    var unlocks = _locks.Where(x => x.Item.Id == id).ToList();
                    _locks.RemoveAll(x => unlocks.Contains(x));

                    foreach (var unlock in unlocks)
                    {
                        Dfs(unlock);
                    }
                }

                if (v2.Item is LockRoom && !_keys.Contains(id))
                {
                    _locks.Add(v2);
                    continue;
                }

                Dfs(v2);
            }
        }
    }
}