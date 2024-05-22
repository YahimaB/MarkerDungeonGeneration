using System.Collections.Generic;
using Generation;

namespace Graph.GeneticSearch.Raters
{
    public class GraphComponentRater : GeneticRater
    {
        public override GeneticRaterMode Mode => GeneticRaterMode.Multiply;
        public override float Weight => 1f;

        private readonly Dictionary<Vertex, HashSet<Vertex>> _graph = new();
        private readonly HashSet<Vertex> _used = new();
        private readonly List<Vertex> _comp = new();

        public override double Rate(GenerationContext context, bool[] sln)
        {
            _graph.Clear();
            _used.Clear();
            _comp.Clear();

            return CountComponents(context, sln);
        }

        public double CountComponents(GenerationContext context, bool[] sln)
        {
            var edges = context.DeloneGraph.Edges;
            var vertices = context.DeloneGraph.Vertices;
            foreach (var vertex in vertices)
            {
                _graph[vertex] = new HashSet<Vertex>();
            }

            for (var i = 0; i < edges.Count; i++)
            {
                if (sln[i])
                {
                    var edge = edges[i];
                    _graph[edge.V].Add(edge.U);
                    _graph[edge.U].Add(edge.V);
                }
            }

            var compCount = 0;
            foreach (var v in vertices)
            {
                if (!_used.Contains(v))
                {
                    _comp.Clear();
                    Dfs(v);
                    compCount++;
                }
            }

            return compCount == 1 ? 0 : FailureValue;
        }

        private void Dfs(Vertex v)
        {
            _used.Add(v);
            _comp.Add(v);
            foreach (var v2 in _graph[v])
            {
                if (!_used.Contains(v2))
                {
                    Dfs(v2);
                }
            }
        }
    }
}