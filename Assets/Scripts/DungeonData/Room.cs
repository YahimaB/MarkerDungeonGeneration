using System;
using System.Collections.Generic;
using DungeonControls;
using UnityEngine;
using Utils;

namespace DungeonData
{
    [Serializable]
    public abstract class Room
    {
        // public RoomType Type { get; set; } = RoomType.None;
        [SerializeField]
        private string _id;

        public virtual string IconName => "";
        public virtual Color Color => new(1f, 0f, 0f, 1f);

        private List<GenerationAction> _preSetupActions = new List<GenerationAction>();
        private List<GenerationAction> _preTreeBuildActions = new List<GenerationAction>();
        private List<GenerationAction> _postGenValidateActions = new List<GenerationAction>();

        public string Id => _id;
        public BoundsInt Bounds { get; set; }
        
        public Vector3Int TargetSize { get; set; }

        public Room() : this(Vector3Int.zero, Vector3Int.one) { }

        public Room(Vector3Int location, Vector3Int size)
        {
            Bounds = new BoundsInt(location, size);
        }

        public static bool Intersect(Room a, Room b)
        {
            return a.Bounds.Intersects(b.Bounds);
        }

        public Room WithPreSetupActions(IEnumerable<GenerationAction> actions)
        {
            _preSetupActions.Clear();
            _preSetupActions.AddRange(actions);
            _preSetupActions.ForEach(x => x.HostRoom = this);
            return this;
        }

        public Room WithPreTreeBuildActions(IEnumerable<GenerationAction> actions)
        {
            _preTreeBuildActions.Clear();
            _preTreeBuildActions.AddRange(actions);
            _preTreeBuildActions.ForEach(x => x.HostRoom = this);
            return this;
        }

        public Room WithPostGenValidateActions(IEnumerable<GenerationAction> actions)
        {
            _postGenValidateActions.Clear();
            _postGenValidateActions.AddRange(actions);
            _postGenValidateActions.ForEach(x => x.HostRoom = this);
            return this;
        }

        public void PreSetup()
        {
            foreach (var action in _preSetupActions)
            {
                action.Run();
            }
        }

        public void PreTreeBuild()
        {
            foreach (var action in _preTreeBuildActions)
            {
                action.Run();
            }
        }

        public void PostGenValidate()
        {
            foreach (var action in _postGenValidateActions)
            {
                action.Run();
            }
        }
    }

    // public enum RoomType
    // {
    //     None = 0,
    //
    //     Enter = 1,
    //     Exit = 2,
    //
    //     Key = 11,
    //     Locked = 12,
    // }

    [Serializable]
    public class EnterRoom : Room
    {
        public override string IconName => "enter.png";
        public override Color Color => new(0f, 1f, 0f, 0.5f);

        public EnterRoom() : base() { }

        public EnterRoom(Vector3Int location, Vector3Int size) : base(location, size) { }
    }

    [Serializable]
    public class ExitRoom : Room
    {
        public override string IconName => "exit.png";
        public override Color Color => new(1f, 0f, 0f, 0.5f);

        public ExitRoom() : base() { }

        public ExitRoom(Vector3Int location, Vector3Int size) : base(location, size) { }
    }

    [Serializable]
    public class BossRoom : Room
    {
        public override string IconName => "boss.png";
        public override Color Color => new(0f, 0f, 1f, 0.5f);

        public BossRoom() : base() { }

        public BossRoom(Vector3Int location, Vector3Int size) : base(location, size) { }
    }
    
    [Serializable]
    public class KeyRoom : Room
    {
        public override string IconName => "key.png";
        public override Color Color => new(1f, 1f, 0.5f, 0.5f);

        public KeyRoom() : base() { }

        public KeyRoom(Vector3Int location, Vector3Int size) : base(location, size) { }
    }
    
    [Serializable]
    public class LockRoom : Room
    {
        public override string IconName => "lock.png";
        public override Color Color => new(1f, 1f, 0.5f, 0.5f);

        public LockRoom() : base() { }

        public LockRoom(Vector3Int location, Vector3Int size) : base(location, size) { }
    }
}