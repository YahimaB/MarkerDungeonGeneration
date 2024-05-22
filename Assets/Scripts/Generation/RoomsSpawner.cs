using System;
using System.Collections.Generic;
using DungeonControls;
using DungeonData;
using Generation;
using UnityEngine;
using Utils;

public class RoomsSpawner : MonoBehaviour
{
    [SerializeField]
    private bool _showDebug;

    [Header("Spawn")]
    [SerializeField]
    private int _extraRoomCount;
    [SerializeField]
    private int _minSpawnRadius;
    [SerializeField]
    private float _radiusOffsetMultiplier;
    [SerializeField]
    private float _radiusIntersectMultiplier;
    [SerializeField]
    private int _maxSpawnTries = 5;

    [Header("Growth")]
    [SerializeField]
    private Vector3Int _roomMinSize;
    [SerializeField]
    private Vector3Int _roomMaxSize;
    [SerializeField]
    private Vector3Int _interiorSpace;
    [SerializeField]
    private int _growthSteps;

    public GenerationContext Context { get; private set; }
    private List<Room> Rooms => Context.Rooms;

    private Grid3D<bool> _fillGrid = null;

    // private readonly List<Room> _rooms = new List<Room>();

    private void Awake()
    {
        Context = null;
    }

    public static List<Room> GetCurrentRooms()
    {
        var self = FindObjectOfType<RoomsSpawner>();
        return self.Rooms;
    }

    public void PlaceRooms(GenerationContext context)
    {
        Context = context;

        Context.InitializeRooms();
        _fillGrid = new Grid3D<bool>(Context.GridSize);

        SpawnInitRooms();
        SpawnExtraRooms();

        // SpawnRoomsSeeds(Context.GridSize);
        // // return;
        //
        // var targetSizes = new List<Vector3Int>();
        // for (var i = 0; i < Rooms.Count; i++)
        // {
        //     var targetSize = new Vector3Int(
        //         GenRandom.Next(_roomMinSize.x, _roomMaxSize.x),
        //         GenRandom.Next(_roomMinSize.y, _roomMaxSize.y),
        //         GenRandom.Next(_roomMinSize.z, _roomMaxSize.z)
        //     );
        //     // var targetSize = _roomMaxSize;
        //     targetSizes.Add(targetSize);
        // }
        //
        // for (var step = 0; step < _growthSteps; step++)
        // {
        //     for (var index = 0; index < Rooms.Count; index++)
        //     {
        //         GrowRoomStep(Rooms, index, targetSizes[index], Context.GridSize);
        //     }
        // }


        Context.MarkRoomsCells();
    }

    private void SpawnInitRooms()
    {
        var initialRooms = new List<Room>();
        var targetSizes = new List<Vector3Int>();

        var roomNodes = FindObjectsOfType<RoomNode>();
        foreach (var roomNode in roomNodes)
        {
            var room = roomNode.GenerateRoom();
            // _fillGrid[room.Bounds.position] = true;

            initialRooms.Add(room);

            var targetSize = roomNode.GetTargetSize() != Vector3Int.zero
                ? roomNode.GetTargetSize()
                : RandomizeSize();

            targetSizes.Add(targetSize);
            room.TargetSize = targetSize;
        }

        Rooms.AddRange(initialRooms);
        for (var step = 0; step < _growthSteps; step++)
        {
            for (var index = 0; index < initialRooms.Count; index++)
            {
                GrowRoomStep(Rooms, index, targetSizes[index], Context.GridSize);
            }
        }

        foreach (var room in initialRooms)
        {
            var bounds = room.Bounds;
            var newBounds = new BoundsInt(bounds.position + _interiorSpace / -2, bounds.size + _interiorSpace);

            foreach (var pos in newBounds.allPositionsWithin)
            {
                if (_fillGrid.InBounds(pos))
                {
                    _fillGrid[pos] = true;
                }
            }
        }
    }

    private void SpawnExtraRooms()
    {
        var initRoomsCount = Rooms.Count;

        var randomRooms = SpawnRandomRooms(_fillGrid);
        Rooms.AddRange(randomRooms);

        var targetSizes = new List<Vector3Int>();
        for (var i = 0; i < randomRooms.Count; i++)
        {
            var targetSize = RandomizeSize();
            // var targetSize = _roomMaxSize;
            targetSizes.Add(targetSize);
            randomRooms[i].TargetSize = targetSize;
        }

        for (var step = 0; step < _growthSteps; step++)
        {
            for (var index = 0; index < randomRooms.Count; index++)
            {
                GrowRoomStep(Rooms, initRoomsCount + index, targetSizes[index], Context.GridSize);
            }
        }
    }

    #region SpawnRoomSeeds

    [Obsolete]
    private void SpawnRoomsSeeds(Vector3Int gridSize)
    {
        var initialRooms = SpawnInitialRooms(_fillGrid);
        var randomRooms = SpawnRandomRooms(_fillGrid);

        Rooms.AddRange(initialRooms);
        Rooms.AddRange(randomRooms);
    }

    [Obsolete]
    private List<Room> SpawnInitialRooms(in Grid3D<bool> fillGrid)
    {
        var initialRooms = new List<Room>();

        var roomNodes = FindObjectsOfType<RoomNode>();
        foreach (var roomNode in roomNodes)
        {
            var room = roomNode.GenerateRoom();

            PlaceRoomSeed(fillGrid, room.Bounds.position, false);
            initialRooms.Add(room);
        }

        return initialRooms;
    }

    private List<Room> SpawnRandomRooms(in Grid3D<bool> fillGrid)
    {
        var size = fillGrid.Size;
        var roomSeeds = new List<Vector3Int>();

        var roomsToSpawn = _extraRoomCount;
        int offset = _minSpawnRadius <= 1 ? 1 : Mathf.RoundToInt(_minSpawnRadius * _radiusOffsetMultiplier);

        for (var i = 0; i < roomsToSpawn; i++)
        {
            var triesCount = 0;
            while (triesCount < _maxSpawnTries)
            {
                triesCount++;

                var pos = new Vector3Int(
                    GenRandom.Next(offset, size.x - offset),
                    GenRandom.Next(offset, size.y - offset),
                    GenRandom.Next(offset, size.z - offset)
                );

                if (PlaceRoomSeed(fillGrid, pos, true))
                {
                    roomSeeds.Add(pos);
                    break;
                }
            }
        }

        var randomRooms = new List<Room>();
        foreach (var roomSeed in roomSeeds)
        {
            Room newRoom = new EmptyRoom(roomSeed, Vector3Int.one);
            randomRooms.Add(newRoom);
        }

        return randomRooms;
    }

    private bool PlaceRoomSeed(in Grid3D<bool> fillGrid, Vector3Int pos, bool withCheck)
    {
        var size = fillGrid.Size;
        if (withCheck && fillGrid[pos])
            return false;

        var minRadius = Mathf.RoundToInt(_minSpawnRadius * _radiusIntersectMultiplier);
        var sqrRadius = minRadius * minRadius;

        for (var x = Math.Max(0, pos.x - minRadius); x <= Math.Min(size.x - 1, pos.x + minRadius); x++)
        {
            for (var y = Math.Max(0, pos.y - minRadius); y <= Math.Min(size.y - 1, pos.y + minRadius); y++)
            {
                for (var z = Math.Max(0, pos.z - minRadius); z <= Math.Min(size.z - 1, pos.z + minRadius); z++)
                {
                    var x2 = (x - pos.x) * (x - pos.x);
                    var y2 = (y - pos.y) * (y - pos.y);
                    var z2 = (z - pos.z) * (z - pos.z);

                    if (x2 + y2 + z2 <= sqrRadius)
                    {
                        fillGrid[new Vector3Int(x, y, z)] = true;
                    }
                }
            }
        }

        return true;
    }

    #endregion SpawnRoomSeeds

    private void GrowRoomStep(List<Room> rooms, int index, Vector3Int targetSize, Vector3Int maxValues)
    {
        var room = rooms[index];
        var bounds = room.Bounds;

        var newRoom = new EmptyRoom(bounds.position, bounds.size + new Vector3Int(1, 0, 0));
        if (bounds.size.x < targetSize.x && TryGrowRoom(rooms, newRoom, room, maxValues))
        {
            bounds = newRoom.Bounds;
        }

        newRoom = new EmptyRoom(bounds.position - new Vector3Int(1, 0, 0), bounds.size + new Vector3Int(1, 0, 0));
        if (bounds.size.x < targetSize.x && TryGrowRoom(rooms, newRoom, room, maxValues))
        {
            bounds = newRoom.Bounds;
        }

        newRoom = new EmptyRoom(bounds.position, bounds.size + new Vector3Int(0, 1, 0));
        if (bounds.size.y < targetSize.y && TryGrowRoom(rooms, newRoom, room, maxValues))
        {
            bounds = newRoom.Bounds;
        }
        newRoom = new EmptyRoom(bounds.position - new Vector3Int(0, 1, 0), bounds.size + new Vector3Int(0, 1, 0));
        if (bounds.size.y < targetSize.y && TryGrowRoom(rooms, newRoom, room, maxValues))
        {
            bounds = newRoom.Bounds;
        }

        newRoom = new EmptyRoom(bounds.position, bounds.size + new Vector3Int(0, 0, 1));
        if (bounds.size.z < targetSize.z && TryGrowRoom(rooms, newRoom, room, maxValues))
        {
            bounds = newRoom.Bounds;
        }

        newRoom = new EmptyRoom(bounds.position - new Vector3Int(0, 0, 1), bounds.size + new Vector3Int(0, 0, 1));
        if (bounds.size.z < targetSize.z && TryGrowRoom(rooms, newRoom, room, maxValues))
        {
            bounds = newRoom.Bounds;
        }

        rooms[index].Bounds = bounds;
    }

    private bool TryGrowRoom(List<Room> rooms, Room newRoom, Room oldRoom, Vector3Int maxValues)
    {
        var bounds = newRoom.Bounds;
        var pos = bounds.position;
        var size = bounds.size;

        bool outOfRange = pos.x < 0 || pos.x + size.x > maxValues.x ||
                          pos.y < 0 || pos.y + size.y > maxValues.y ||
                          pos.z < 0 || pos.z + size.z > maxValues.z;
        if (outOfRange)
        {
            return false;
        }

        newRoom = new EmptyRoom(bounds.position + _interiorSpace / -2, bounds.size + _interiorSpace);
        foreach (var otherRoom in rooms)
        {
            if (otherRoom == oldRoom)
                continue;
            if (Room.Intersect(newRoom, otherRoom))
                return false;
        }

        return true;
    }

    private Vector3Int RandomizeSize()
    {
        return new Vector3Int(
            GenRandom.Next(_roomMinSize.x, _roomMaxSize.x),
            GenRandom.Next(_roomMinSize.y, _roomMaxSize.y),
            GenRandom.Next(_roomMinSize.z, _roomMaxSize.z)
        );
    }

    private void OnDrawGizmos()
    {
        if (!_showDebug)
            return;
        
        var offset = Vector3.one * 0.5f;
        if (_fillGrid != null)
        {
            var size = _fillGrid.Size;
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    for (int z = 0; z < size.z; z++)
                    {
                        var point = new Vector3Int(x, y, z);
                        if (_fillGrid[point])
                        {
                            Gizmos.color = new Color(0.5f, 0.3f, 0.5f, 0.8f);
                            Gizmos.DrawSphere(point + offset, 0.3f);
                        }
                    }
                }
            }
        }
    }
}