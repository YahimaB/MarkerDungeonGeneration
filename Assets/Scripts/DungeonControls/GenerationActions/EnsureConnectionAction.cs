using System;
using DungeonData;
using Graph;
using Graph.GeneticSearch;
using UnityEngine;
using Utils.ClassTypeReference;

namespace DungeonControls
{
    [Serializable]
    public class EnsureConnectionAction : GenerationAction
    {
        [SerializeField]
        private RoomNode _targetNode = null;

        [SerializeField]
        private bool _removeOthers;

        public override void Run()
        {
            var targetRoom = _targetNode?.GetGeneratedRoom();
            if (targetRoom == null)
            {
                return;
            }
            
            var context = GetContext();
            
            var selfVertex = context.DeloneGraph.Vertices.Find(x => (x as Vertex<Room>)?.Item == HostRoom);
            var targetVertex = context.DeloneGraph.Vertices.Find(x => (x as Vertex<Room>)?.Item == targetRoom);

            if (_removeOthers)
            {
                context.DeloneGraph.Edges.RemoveAll(e => e.V == selfVertex || e.U == selfVertex);
            }
            else
            {
                foreach (var edge in context.DeloneGraph.Edges)
                {
                    if (edge.V == selfVertex && edge.U == targetVertex || edge.U == selfVertex && edge.V == targetVertex)
                    {
                        context.DeloneGraph.Edges.Remove(edge);
                    
                        break;
                    }
                } 
            }

            context.DeloneGraph.Edges.Add(new PermanentEdge(selfVertex, targetVertex));
        }
    }
}