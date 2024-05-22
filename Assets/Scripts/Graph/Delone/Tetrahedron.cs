using System;
using UnityEngine;

namespace Graph.Delone
{
    public class Tetrahedron : IEquatable<Tetrahedron>
    {
        public Vertex A { get; set; }
        public Vertex B { get; set; }
        public Vertex C { get; set; }
        public Vertex D { get; set; }

        public bool IsBad { get; set; }

        private Vector3 Circumcenter { get; set; }
        private float CircumradiusSquared { get; set; }

        public Tetrahedron(Vertex a, Vertex b, Vertex c, Vertex d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            CalculateCircumsphere();
        }

        private void CalculateCircumsphere()
        {
            //calculate the circumsphere of a tetrahedron
            //http://mathworld.wolfram.com/Circumsphere.html

            var a = new Matrix4x4(
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z),
                new Vector4(1, 1, 1, 1)
            ).determinant;

            var aPosSqr = A.Position.sqrMagnitude;
            var bPosSqr = B.Position.sqrMagnitude;
            var cPosSqr = C.Position.sqrMagnitude;
            var dPosSqr = D.Position.sqrMagnitude;

            var Dx = new Matrix4x4(
                new Vector4(aPosSqr, bPosSqr, cPosSqr, dPosSqr),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z),
                new Vector4(1, 1, 1, 1)
            ).determinant;

            var Dy = -new Matrix4x4(
                new Vector4(aPosSqr, bPosSqr, cPosSqr, dPosSqr),
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z),
                new Vector4(1, 1, 1, 1)
            ).determinant;

            var Dz = new Matrix4x4(
                new Vector4(aPosSqr, bPosSqr, cPosSqr, dPosSqr),
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                new Vector4(1, 1, 1, 1)
            ).determinant;

            var c = new Matrix4x4(
                new Vector4(aPosSqr, bPosSqr, cPosSqr, dPosSqr),
                new Vector4(A.Position.x, B.Position.x, C.Position.x, D.Position.x),
                new Vector4(A.Position.y, B.Position.y, C.Position.y, D.Position.y),
                new Vector4(A.Position.z, B.Position.z, C.Position.z, D.Position.z)
            ).determinant;

            Circumcenter = new Vector3(
                Dx / (2 * a),
                Dy / (2 * a),
                Dz / (2 * a)
            );

            CircumradiusSquared = (Dx * Dx + Dy * Dy + Dz * Dz - 4 * a * c) / (4 * a * a);
        }

        public bool ContainsVertex(Vertex v)
        {
            return Vertex.AlmostEqual(v, A)
                   || Vertex.AlmostEqual(v, B)
                   || Vertex.AlmostEqual(v, C)
                   || Vertex.AlmostEqual(v, D);
        }

        public bool CircumCircleContains(Vector3 v)
        {
            Vector3 dist = v - Circumcenter;
            return dist.sqrMagnitude <= CircumradiusSquared;
        }

        public static bool operator ==(Tetrahedron left, Tetrahedron right)
        {
            return (left.A == right.A || left.A == right.B || left.A == right.C || left.A == right.D)
                   && (left.B == right.A || left.B == right.B || left.B == right.C || left.B == right.D)
                   && (left.C == right.A || left.C == right.B || left.C == right.C || left.C == right.D)
                   && (left.D == right.A || left.D == right.B || left.D == right.C || left.D == right.D);
        }

        public static bool operator !=(Tetrahedron left, Tetrahedron right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj is Tetrahedron t)
                return this == t;

            return false;
        }

        public bool Equals(Tetrahedron t)
        {
            return this == t;
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode() ^ C.GetHashCode() ^ D.GetHashCode();
        }
    }
}