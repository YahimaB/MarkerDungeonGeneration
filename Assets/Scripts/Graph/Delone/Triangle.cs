namespace Graph.Delone
{
    public class Triangle
    {
        public Vertex U { get; set; }
        public Vertex V { get; set; }
        public Vertex W { get; set; }

        public bool IsBad { get; set; }

        public Triangle() { }

        public Triangle(Vertex u, Vertex v, Vertex w)
        {
            U = u;
            V = v;
            W = w;
        }

        public static bool operator ==(Triangle left, Triangle right)
        {
            return (left.U == right.U || left.U == right.V || left.U == right.W)
                   && (left.V == right.U || left.V == right.V || left.V == right.W)
                   && (left.W == right.U || left.W == right.V || left.W == right.W);
        }

        public static bool operator !=(Triangle left, Triangle right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Triangle e)
                return this == e;

            return false;
        }

        public bool Equals(Triangle e)
        {
            return this == e;
        }

        public override int GetHashCode()
        {
            return U.GetHashCode() ^ V.GetHashCode() ^ W.GetHashCode();
        }

        public static bool AlmostEqual(Triangle left, Triangle right)
        {
            return (Vertex.AlmostEqual(left.U, right.U) || Vertex.AlmostEqual(left.U, right.V) || Vertex.AlmostEqual(left.U, right.W))
                   && (Vertex.AlmostEqual(left.V, right.U) || Vertex.AlmostEqual(left.V, right.V) || Vertex.AlmostEqual(left.V, right.W))
                   && (Vertex.AlmostEqual(left.W, right.U) || Vertex.AlmostEqual(left.W, right.V) || Vertex.AlmostEqual(left.W, right.W));
        }
    }
}