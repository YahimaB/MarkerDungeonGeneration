using System;
using UnityEngine;

namespace DungeonData
{
    public interface IGrid3D
    {
        Vector3Int Size { get; }

        int GetIndex(Vector3Int pos);
        Vector3Int GetPosition(int index);
        bool InBounds(Vector3Int pos);
    }

    public class Grid3D<T> : IGrid3D
    {
        public int Length => Data.Length;
        public Vector3Int Size { get; }
        public T[] Data { get; }

        public Grid3D(Vector3Int size)
        {
            Size = size;

            Data = new T[size.x * size.y * size.z];
        }

        public T this[int x, int y, int z]
        {
            get => this[new Vector3Int(x, y, z)];
            set => this[new Vector3Int(x, y, z)] = value;
        }

        public T this[Vector3Int pos]
        {
            get => Data[GetIndex(pos)];
            set => Data[GetIndex(pos)] = value;
        }

        public int GetIndex(Vector3Int pos)
        {
            return pos.x + Size.x * pos.y + Size.x * Size.y * pos.z;
        }

        public Vector3Int GetPosition(int index)
        {
            var x = index % Size.x;
            var y = (index / Size.x) % Size.y;
            var z = index / (Size.x * Size.y);

            return new Vector3Int(x, y, z);
        }

        public bool InBounds(Vector3Int pos)
        {
            return new BoundsInt(Vector3Int.zero, Size).Contains(pos);
        }

        public void DoForEach(Action<T> action)
        {
            for (var x = 0; x < Size.x; x++)
            {
                for (var y = 0; y < Size.y; y++)
                {
                    for (var z = 0; z < Size.z; z++)
                    {
                        var node = this[x, y, z];
                        action?.Invoke(node);
                    }
                }
            }
        }
    }
}