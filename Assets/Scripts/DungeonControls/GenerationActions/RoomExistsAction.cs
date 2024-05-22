using System;
using DungeonData;
using UnityEngine;
using Utils.ClassTypeReference;

namespace DungeonControls
{
    [Serializable]
    public class RoomExistsAction : GenerationAction
    {
        [SerializeField]
        [ClassExtends(typeof(Room))]
        private ClassTypeReference _roomType;

        [SerializeField]
        private string _id = "";

        public override void Run()
        {
            var rooms = RoomsSpawner.GetCurrentRooms();
            if (rooms.Exists(r => r.GetType() == _roomType.Type && (string.IsNullOrWhiteSpace(_id) || r.Id == _id)))
            {
                return;
            }

            Debug.LogError($"No room of type {_roomType.Type} with id = {_id} exists");
        }
    }
}