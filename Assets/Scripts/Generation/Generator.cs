using System;
using System.Collections.Generic;
using System.Linq;
using DungeonData;
using Graph.GeneticSearch;
using Graph.GeneticSearch.Raters;
using UnityEngine;
using Utils;
using Utils.ClassTypeReference;

namespace Generation
{
    public class Generator : MonoBehaviour
    {
        [SerializeField]
        private GameObject _cubePrefab;
        [SerializeField]
        private RoomsSpawner _roomsSpawner;

        [SerializeField]
        private Vector3Int _spaceSize;

        [SerializeField]
        public List<GeneticRaterHolder> _geneticRaters = new List<GeneticRaterHolder>();

        private GenerationContext Context { get; set; }
        private Grid3D<Cell> Grid => Context.Grid;

        private void OnValidate()
        {
            foreach (var rater in _geneticRaters)
            {
                ValidateTypedObject(rater.RaterType, ref rater.Rater);
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

        public void StartGeneration(bool drawDungeon)
        {
            Context = new GenerationContext(_spaceSize);
            GenRandom.Initialize();
            
            _roomsSpawner.PlaceRooms(Context);

            GraphBuilder.BuildGraph(Context, GetGeneticRaters(), drawDungeon);

            HallwaysBuilder.PathfindHallways(Context);
            Context.PostGenValidation();

            if (drawDungeon)
            {
                DrawDungeon();
            }
        }

        private List<GeneticRater> GetGeneticRaters()
        {
            return _geneticRaters.Select(x => x.Rater).ToList();
        }

        private void DrawDungeon()
        {
            foreach (var room in Context.Rooms)
            {
                PlaceRoom(room.Bounds.position, room.Bounds.size);
            }

            var size = Grid.Size;
            for (var x = 0; x < size.x; x++)
            {
                for (var y = 0; y < size.y; y++)
                {
                    for (var z = 0; z < size.z; z++)
                    {
                        var pos = new Vector3Int(x, y, z);
                        switch (Grid[pos].Type)
                        {
                            case CellType.Hallway:
                                PlaceHallway(pos);
                                break;
                        }
                    }
                }
            }
        }

        private void PlaceCube(Vector3Int location, Vector3Int size, Color color)
        {
            var go = Instantiate(_cubePrefab, location, Quaternion.identity);
            go.GetComponent<Transform>().localScale = size;
            go.GetComponent<MeshRenderer>().material.color = color;
        }

        private void PlaceRoom(Vector3Int location, Vector3Int size)
        {
            PlaceCube(location, size, Color.white);
        }

        private void PlaceHallway(Vector3Int location)
        {
            PlaceCube(location, new Vector3Int(1, 1, 1), Color.blue);
        }

        private void OnDrawGizmos()
        {
            // var size = Vector3.one;
            var pos =  Vector3Int.FloorToInt(transform.position);
            
            // var pos = GetBoundsPos();
            var center = pos + (Vector3)_spaceSize / 2f;

            var sizeOffset = Vector3.one * 0.005f;
            Gizmos.color = new Color(0.1f, 0.1f, 0.1f, 0.2f);
            Gizmos.DrawCube(center, _spaceSize + sizeOffset);
            
            Gizmos.color = new Color(0f, 0f, 0f, 1f);
            Gizmos.DrawWireCube(center, _spaceSize);
        }
    }
}