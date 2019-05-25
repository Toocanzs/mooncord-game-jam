using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Tilemaps.Tile;
namespace ThetaStar
{
    public class Vertex
    {
        private List<Vertex> neighbors = new List<Vertex>(8);
        public List<Vertex> Neighbors => neighbors;
        public Vector3Int position;
        public ColliderType colliderType;
        public Vertex(Vector3Int position, ColliderType colliderType)
        {
            this.position = position;
            this.colliderType = colliderType;
        }

        public void AddNeighbor(Vertex neighbor)
        {
            neighbors.Add(neighbor);
        }
    }
    public class PathfindingGenerator : MonoBehaviour
    {
        [SerializeField]
        private GameObject testObject;
        [SerializeField]
        private Tilemap[] tilemaps;

        Vector3Int tilemapsStart = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
        Vector3Int tilemapsEnd = Vector3Int.zero;
        Vector3Int tilemapSize = Vector3Int.zero;

        Tuple<TileBase, ColliderType?> GetTile(Vector3Int pos)
        {
            TileBase outputTile = null;
            ColliderType? colliderType = null;
            foreach (Tilemap tilemap in tilemaps)
            {
                TileBase tile = tilemap.GetTile(pos);
                ColliderType type = tilemap.GetColliderType(pos);
                if (outputTile == null)
                {
                    outputTile = tile;
                    if (tile == null)
                        colliderType = null;
                    else
                        colliderType = type;
                }
                else if (type > colliderType)//tiles with colliders take priority.
                {
                    outputTile = tile;
                    colliderType = type;
                }
            }
            return new Tuple<TileBase, ColliderType?>(outputTile, colliderType);
        }

        private Dictionary<Vector3Int, Vertex> postionToVertex = new Dictionary<Vector3Int, Vertex>();

        void Start()
        {
            foreach (Tilemap tilemap in tilemaps)
            {
                tilemapsStart = Vector3Int.Min(tilemap.cellBounds.position, tilemapsStart);
                tilemapsEnd = Vector3Int.Max(tilemap.cellBounds.position + tilemap.cellBounds.size, tilemapsEnd);
            }
            tilemapSize = tilemapsEnd - tilemapsStart;
            
            CreateVertices();
            SetupNeighbors();
            Debug.Log(postionToVertex[tilemapsStart + new Vector3Int(1,1,0)].Neighbors.Count);
        }

        private void SetupNeighbors()
        {
            //setup neighbors pass
            for (int y = 0; y < tilemapSize.y; y++)
            {
                for (int x = 0; x < tilemapSize.x; x++)
                {
                    Vector3Int pos = tilemapsStart + new Vector3Int(x, y, 0);

                    if (postionToVertex.TryGetValue(pos, out Vertex vertex))
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            for (int i = -1; i <= 1; i++)
                            {
                                if (i == 0 && j == 0)
                                    continue;
                                if (postionToVertex.TryGetValue(pos + new Vector3Int(i, j, 0), out Vertex neighbor))
                                {
                                    if(neighbor.colliderType == ColliderType.None)
                                        vertex.AddNeighbor(neighbor);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreateVertices()
        {
            //create verticies pass
            for (int y = 0; y < tilemapSize.y; y++)
            {
                for (int x = 0; x < tilemapSize.x; x++)
                {
                    Vector3Int pos = tilemapsStart + new Vector3Int(x, y, 0);

                    var tile = GetTile(pos);
                    if (tile.Item1 != null)
                    {
                        Vertex vertex = new Vertex(pos, tile.Item2.Value);
                        postionToVertex[pos] = vertex;
                    }
                }
            }
        }

        void Update()
        {

        }
    }

}