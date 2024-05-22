using System;
using DungeonData;
using Graph;
using UnityEngine;
using Utils.ClassTypeReference;

namespace DungeonControls
{
    [Serializable]
    public class ConnectionExistsAction : GenerationAction
    {
        [SerializeField]
        [ClassExtends(typeof(Room))]
        private ClassTypeReference _roomType;

        [SerializeField]
        private string _id = "";

        public override void Run()
        {
            var context = GetContext();

            foreach (var edge in context.SpanningTree)
            {
                Room roomToCheck = null;
                if ((edge.V as Vertex<Room>)?.Item == HostRoom)
                {
                    roomToCheck = (edge.U as Vertex<Room>)?.Item;
                }
                else if ((edge.U as Vertex<Room>)?.Item == HostRoom)
                {
                    roomToCheck = (edge.V as Vertex<Room>)?.Item;
                }

                if (roomToCheck != null && roomToCheck.GetType() == _roomType.Type && (string.IsNullOrWhiteSpace(_id) || roomToCheck.Id == _id))
                {
                    return;
                }
            }

            Debug.LogError($"No connection between {HostRoom?.GetType()} ({HostRoom?.Id}) and {_roomType.Type} ({_id}) exists");
        }
    }
}