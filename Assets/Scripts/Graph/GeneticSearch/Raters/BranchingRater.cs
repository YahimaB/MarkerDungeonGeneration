using System.Collections.Generic;
using Generation;
using UnityEngine;

namespace Graph.GeneticSearch.Raters
{
    public class BranchingRater : GeneticRater
    {
        [SerializeField]
        private float _weight = 10f;
        
        [SerializeField]
        [Range(0, 100)]
        private int _branchingRate;

        public override float Weight => _weight;
        public override GeneticRaterMode Mode => GeneticRaterMode.Add;


        public override double Rate(GenerationContext context, bool[] sln)
        {
            var edges = context.DeloneGraph.Edges;
            var vEdgeCount = new Dictionary<Vertex, int>();

            for (var i = 0; i < edges.Count; i++)
            {
                if (!sln[i])
                {
                    continue;
                }

                var edge = edges[i];

                vEdgeCount.TryAdd(edge.V, 0);
                vEdgeCount.TryAdd(edge.U, 0);

                vEdgeCount[edge.V] += 1;
                vEdgeCount[edge.U] += 1;
            }

            var endNodes = 0;
            var nonLinearNodes = 0;
            foreach (var edgeCount in vEdgeCount.Values)
            {
                if (endNodes < 2 && edgeCount == 1)
                {
                    endNodes++;
                    continue;
                }

                if (edgeCount != 2)
                {
                    nonLinearNodes++;
                }
            }

            var branching = (float)nonLinearNodes / (vEdgeCount.Count - endNodes);

            var targetBranching = _branchingRate / 100f;
            var deviation = Mathf.Abs(branching - targetBranching);

            return deviation;
        }
    }
}