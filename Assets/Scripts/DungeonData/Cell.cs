using UnityEngine;

namespace DungeonData
{
    public class Cell
    {
        public CellType Type { get; set; } = CellType.None;
        public Vector3Int Position { get; }

        public Cell(Vector3Int position)
        {
            Position = position;
        }
    }

    public enum CellType
    {
        None = 0,
        Room = 1,
        Hallway = 2,
    }
}