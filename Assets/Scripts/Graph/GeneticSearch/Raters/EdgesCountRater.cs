using System.Linq;
using Generation;
using UnityEngine;

namespace Graph.GeneticSearch.Raters
{
    public class EdgesCountRater : GeneticRater
    {
        [SerializeField]
        private float _weight = 20f;

        [SerializeField]
        [Range(0, 100)]
        private int _edgesFilling;
        
        public override float Weight => _weight;
        public override GeneticRaterMode Mode => GeneticRaterMode.Add;

        public override double Rate(GenerationContext context, bool[] sln)
        {
            var minEdges = context.DeloneGraph.Vertices.Count - 1;
            var maxEdges = sln.Length;
            var targetEdges = minEdges + (maxEdges - minEdges) * (_edgesFilling / 100f);

            var realEdges = sln.Count(x => x);
            var deviation = Mathf.Abs(realEdges - targetEdges) / targetEdges;
            
            return deviation;
        }
    }
}