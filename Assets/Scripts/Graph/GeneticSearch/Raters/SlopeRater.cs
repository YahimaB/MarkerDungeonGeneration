using System.Collections.Generic;
using System.Linq;
using Generation;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Graph.GeneticSearch.Raters
{
    public class SlopeRater : GeneticRater
    {
        [SerializeField]
        private float _weight = 10f;

        [SerializeField]
        [Range(0, 100)]
        private int _slopeRate;

        public override float Weight => _weight;
        public override GeneticRaterMode Mode => GeneticRaterMode.Add;

        public override double Rate(GenerationContext context, bool[] sln)
        {
            var min = 0.0f;
            var max = 0.0f;

            var edges = context.DeloneGraph.Edges;
            var angles = new List<float>();

            for (var i = 0; i < edges.Count; i++)
            {
                var edge = edges[i];
                var v = edge.V.Position - edge.U.Position;
                var value = Mathf.Abs(90 - Vector3.Angle(v, Vector3.up));

                if (value > max)
                {
                    max = value;
                }

                if (value < min)
                {
                    min = value;
                }

                if (!sln[i])
                {
                    angles.Add(value);
                }
            }

            var range = max - min;
            var targetAngle = _slopeRate / 100f * range + min;

            var rmspe = angles.RMSPE(targetAngle, range);
            // var deviation = rmse / range;

            return rmspe;
        }
    }
}