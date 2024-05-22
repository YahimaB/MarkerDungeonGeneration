using System.Collections.Generic;
using Generation;
using UnityEngine;
using Utils;

namespace Graph.GeneticSearch.Raters
{
    public class EdgeLengthRater : GeneticRater
    {
        [SerializeField]
        private float _weight = 10f;

        [SerializeField]
        [Range(0, 100)]
        private int _lengthRate;

        public override float Weight => _weight;
        public override GeneticRaterMode Mode => GeneticRaterMode.Add;

        public override double Rate(GenerationContext context, bool[] sln)
        {
            var min = 0.0f;
            var max = 0.0f;

            var edges = context.DeloneGraph.Edges;
            var lengths = new List<float>();

            for (var i = 0; i < edges.Count; i++)
            {
                var edge = edges[i];
                var v = edge.V.Position - edge.U.Position;
                var value = v.magnitude;

                if (value > max)
                {
                    max = value;
                }

                if (value < min)
                {
                    min = value;
                }

                if (sln[i])
                {
                    lengths.Add(value);
                }
            }

            var range = max - min;
            var targetLength = _lengthRate / 100f * range + min;

            var rmspe = lengths.RMSPE(targetLength, range);
            // var deviation = rmse / range;

            return rmspe;
        }
    }
}