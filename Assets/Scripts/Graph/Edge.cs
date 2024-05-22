using System;

namespace Graph
{
    public class Edge : IEquatable<Edge>
    {
        public Vertex U { get; }
        public Vertex V { get; }

        public Edge() { }

        public Edge(Vertex u, Vertex v)
        {
            U = u;
            V = v;
        }

        // U - V
        // U - U
        public static bool operator ==(Edge left, Edge right)
        {
            return left.U == right.U && left.V == right.V ||
                   left.V == right.U && left.U == right.V;

            // return (left.U == right.U || left.U == right.V)
            //        && (left.V == right.U || left.V == right.V);
        }

        public static bool operator !=(Edge left, Edge right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Edge e)
                return this == e;

            return false;
        }

        public bool Equals(Edge e)
        {
            return this == e;
        }

        public override int GetHashCode()
        {
            return U.GetHashCode() ^ V.GetHashCode();
        }
    }
}