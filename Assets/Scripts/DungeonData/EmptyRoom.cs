using System;
using UnityEngine;

namespace DungeonData
{
    [Serializable]
    public class EmptyRoom : Room
    {
        public override string IconName => null;
        public override Color Color => new(0.5f, 0.5f, 0.5f, 0.5f);

        public EmptyRoom() : base() { }

        public EmptyRoom(Vector3Int location, Vector3Int size) : base(location, size) { }
    }
}