using UnityEngine;

namespace Graph.Prim
{
    public class PrimEdge : Edge
    {
        public float Distance { get; }

        public PrimEdge(Vertex u, Vertex v) : base(u, v)
        {
            Distance = Vector3.Distance(u.Position, v.Position);
        }
    }
}