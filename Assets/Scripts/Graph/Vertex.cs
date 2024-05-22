using System;
using UnityEngine;

namespace Graph
{
    public class Vertex : IEquatable<Vertex>
    {
        public Vector3 Position { get; }

        public Vertex() { }

        public Vertex(Vector3 position)
        {
            Position = position;
        }
        
        public static bool AlmostEqual(Vertex left, Vertex right)
        {
            return (left.Position - right.Position).sqrMagnitude < 0.01f;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vertex v)
                return Position == v.Position;

            return false;
        }

        public bool Equals(Vertex other)
        {
            return Position == other?.Position;
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }

    public class Vertex<T> : Vertex
    {
        public T Item { get; private set; }

        public Vertex(T item)
        {
            Item = item;
        }

        public Vertex(Vector3 position, T item) : base(position)
        {
            Item = item;
        }
    }
}