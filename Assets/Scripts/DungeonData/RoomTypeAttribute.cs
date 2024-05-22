using System;
using System.Runtime.CompilerServices;

namespace DungeonData
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RoomTypeAttribute : Attribute
    {
        public string Name { get; }

        public RoomTypeAttribute([CallerMemberName] string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"RoomTypeAttribute cannot have empty name");
            }

            Name = name;
        }
    }
}