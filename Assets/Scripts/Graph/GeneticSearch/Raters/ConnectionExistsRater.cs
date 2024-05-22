using DungeonData;
using Generation;
using UnityEngine;

namespace Graph.GeneticSearch.Raters
{
    public class ConnectionExistsRater : GeneticRater
    {
        [SerializeField]
        private float _weight;
        
        public override float Weight => _weight;
        public override GeneticRaterMode Mode => GeneticRaterMode.Add;


        public override double Rate(GenerationContext context, bool[] sln)
        {
            // var d = 0.0;
            
            var edges = context.DeloneGraph.Edges;

            for (var i = 0; i < edges.Count; i++)
            {
                var edge = edges[i];
                if ((edge.V as Vertex<Room>)?.Item.Id == "MainExit" && (edge.U as Vertex<Room>)?.Item.Id == "MainBoss")
                {
                    return sln[i] ? 0.1 : 10;
                }
                if ((edge.U as Vertex<Room>)?.Item.Id == "MainExit" && (edge.V as Vertex<Room>)?.Item.Id == "MainBoss")
                {
                    return sln[i] ? 0.1 : 10;
                }
            }

            return 0;
        }
    }
}