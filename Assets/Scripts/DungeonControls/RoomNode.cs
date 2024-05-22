using System;
using System.Collections.Generic;
using System.Linq;
using DungeonData;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.ClassTypeReference;

namespace DungeonControls
{
    public class RoomNode : MonoBehaviour
    {
        [SerializeField]
        [ClassExtends(typeof(Room))]
        private ClassTypeReference _roomType;

        [SerializeField]
        private Vector3Int _roomSize;

        [SerializeReference]
        private Room _room = null;

        [SerializeField]
        public List<GenerationActionHolder> _preSetupActions = new List<GenerationActionHolder>();

        [SerializeField]
        public List<GenerationActionHolder> _preTreeBuildActions = new List<GenerationActionHolder>();

        [SerializeField]
        public List<GenerationActionHolder> _postGenValidateActions = new List<GenerationActionHolder>();

        private void OnValidate()
        {
            ValidateTypedObject(_roomType, ref _room);

            foreach (var genAction in _preSetupActions)
            {
                ValidateTypedObject(genAction.ActionType, ref genAction.Action);
            }

            foreach (var genAction in _preTreeBuildActions)
            {
                ValidateTypedObject(genAction.ActionType, ref genAction.Action);
            }

            foreach (var genAction in _postGenValidateActions)
            {
                ValidateTypedObject(genAction.ActionType, ref genAction.Action);
            }
        }

        private void ValidateTypedObject<T>(ClassTypeReference typeReference, ref T obj) where T : class
        {
            if (typeReference?.Type == null)
            {
                obj = null;
                return;
            }

            if (typeReference.Type == obj?.GetType())
            {
                return;
            }

            obj = (T)Activator.CreateInstance(typeReference.Type);
        }

        private void OnDrawGizmos()
        {
            var size = Vector3.one;
            var pos = GetBoundsPos();
            var center = pos + size / 2f;

            if (_room != null)
            {
                Gizmos.color = _room.Color;
                Gizmos.DrawIcon(center, _room.IconName, false);
            }
            else
            {
                Gizmos.color = new Color(1f, 0f, 0f, 1f);
            }

            Gizmos.DrawCube(center, size);
        }

        public Vector3 GetBoundsPos()
        {
            return GetBoundsPosInt();
        }

        public Vector3Int GetBoundsPosInt()
        {
            var objPos = transform.position;
            return Vector3Int.FloorToInt(objPos);
        }

        public Vector3Int GetTargetSize()
        {
            return _roomSize;
        }

        public Room GenerateRoom()
        {
            _room.Bounds = new BoundsInt(GetBoundsPosInt(), Vector3Int.one);
            _room.WithPreSetupActions(_preSetupActions.Select(x => x.Action))
                .WithPreTreeBuildActions(_preTreeBuildActions.Select(x => x.Action))
                .WithPostGenValidateActions(_postGenValidateActions.Select(x => x.Action));
            return _room;
        }

        public Room GetGeneratedRoom()
        {
            return _room;
        }
    }
}