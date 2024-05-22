using System.Collections.Generic;
using System.Linq;

namespace Graph.Prim
{
    //TODO: switch to ORIENTED graph build with required edges
    public static class Prim
    {
        public static List<Edge> MinimumSpanningTree(List<Edge> edges, Vertex start)
        {
            var weightedEdges = edges.Select(x=> new PrimEdge(x.U, x.V)).ToList();
            // foreach (var edge in edges)
            // {
            //     weightedEdges.Add(new PrimEdge(edge.U, edge.V));
            // }
            
            var vertices = new HashSet<Vertex>();
            var visitedVertices = new HashSet<Vertex>();

            foreach (var edge in weightedEdges)
            {
                vertices.Add(edge.U);
                vertices.Add(edge.V);
            }

            visitedVertices.Add(start);

            var results = new List<Edge>();

            while (vertices.Count > 0)
            {
                PrimEdge chosenEdge = null;
                var minWeight = float.PositiveInfinity;

                foreach (var edge in weightedEdges)
                {
                    var newVertices = 0;

                    if (visitedVertices.Contains(edge.U))
                        newVertices++;

                    if (visitedVertices.Contains(edge.V))
                        newVertices++;

                    if (newVertices != 1)
                        continue;

                    if (edge.Distance < minWeight)
                    {
                        chosenEdge = edge;
                        minWeight = edge.Distance;
                    }
                }

                if (chosenEdge is null)
                    break;

                results.Add(chosenEdge);
                vertices.Remove(chosenEdge.U);
                vertices.Remove(chosenEdge.V);
                visitedVertices.Add(chosenEdge.U);
                visitedVertices.Add(chosenEdge.V);
            }

            return results;
        }
    }
}