using System.Collections.Generic;
using System.Linq;
using DungeonData;
using Graph;
using Graph.Delone;
using Graph.GeneticSearch;
using Graph.Prim;
using UnityEngine;

namespace Generation
{
    public class GraphBuilder
    {
        public GenerationContext Context { get; private set; }
        public List<GeneticRater> Raters { get; private set; }

        private List<Room> Rooms => Context.Rooms;
        private Delone3D Delone => Context.DeloneGraph;
        private HashSet<Edge> SpanningTree => Context.SpanningTree;

        public static GraphBuilder BuildGraph(GenerationContext context, List<GeneticRater> raters, bool drawDungeon)
        {
            var builder = new GraphBuilder
            {
                Context = context,
                Raters = raters
            };

            builder.Triangulate();
            if (drawDungeon)
            {
                builder.DrawDelaunayLines();
            }

            context.PreTreeBuild();
            // builder.BuildTree();
            builder.BuildGenetic();

            if (drawDungeon)
            {
                builder.DrawTreeLines();
            }

            return builder;
        }

        private void Triangulate()
        {
            var vertices = new List<Vertex>();

            foreach (var room in Rooms)
            {
                var center = room.Bounds.position + (Vector3)room.Bounds.size / 2f;
                var pos = center - new Vector3(0, (room.Bounds.size.y / 2), 0);
                vertices.Add(new Vertex<Room>(pos, room));
            }

            Context.SetDeloneGraph(Delone3D.Triangulate(vertices));
        }

        private void BuildTree()
        {
            var edges = Delone.Edges;
            var minimumSpanningTree = Prim.MinimumSpanningTree(Delone.Edges, edges[0].U);

            Context.SetSpanningTree(minimumSpanningTree.ToHashSet());
        }

        private void BuildGenetic()
        {
            var edges = Delone.Edges;

            var solver = new GeneticSolver(Context, Raters, 10);

            solver.Solve(1000);
            solver.Dump();

            var solution = solver.BestSln;
            var tree = new HashSet<Edge>();
            for (var i = 0; i < edges.Count; i++)
            {
                if (solution[i])
                {
                    tree.Add(edges[i]);
                }
            }

            Context.SetSpanningTree(tree);
        }

        private void DrawDelaunayLines()
        {
            foreach (var edge in Delone.Edges)
            {
                Debug.DrawLine(edge.U.Position, edge.V.Position, Color.red, 5f, false);
            }
        }

        private void DrawTreeLines()
        {
            foreach (var edge in SpanningTree)
            {
                Debug.DrawLine(edge.U.Position, edge.V.Position, Color.black, 1000f, false);
            }
        }
    }
}