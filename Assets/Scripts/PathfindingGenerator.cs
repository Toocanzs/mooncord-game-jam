using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.Tilemaps.Tile;
namespace ThetaStar
{
    public class Vertex : IComparable<Vertex>
    {
        private List<Vertex> neighbors = new List<Vertex>(8);
        public List<Vertex> Neighbors => neighbors;
        public Vector3Int position;
        public ColliderType colliderType;
        public int hValue;
        public int gValue;
        public Vertex(Vector3Int position, ColliderType colliderType)
        {
            this.position = position;
            this.colliderType = colliderType;
        }

        public void AddNeighbor(Vertex neighbor)
        {
            neighbors.Add(neighbor);
        }

        public int CompareTo(Vertex other)//Doesn't really compare properly, more of a heuristic value to be used for comparison
        {
            return hValue + gValue;
        }
        public override bool Equals(object obj)
        {
            if (obj is Vertex v)
                return v.position == this.position;
            return false;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode();
        }
    }
    [DefaultExecutionOrder(-100)]
    public class PathfindingGenerator : MonoBehaviour
    {
        public static PathfindingGenerator Instance;
        [SerializeField]
        private GameObject testObject;
        [SerializeField]
        private Tilemap[] tilemaps;
        [SerializeField]
        private Transform startTest;
        [SerializeField]
        private Transform endTest;
        [SerializeField]
        private LayerMask layerMask;

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

        private Dictionary<Vector3Int, Vertex> positionToVertex = new Dictionary<Vector3Int, Vertex>();

        void Start()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            foreach (Tilemap tilemap in tilemaps)
            {
                tilemapsStart = Vector3Int.Min(tilemap.cellBounds.position, tilemapsStart);
                tilemapsEnd = Vector3Int.Max(tilemap.cellBounds.position + tilemap.cellBounds.size, tilemapsEnd);
            }
            tilemapSize = tilemapsEnd - tilemapsStart;

            CreateVertices();
            SetupNeighbors();
        }

        public int Heuristic(Vector3Int pos, Vector3 end)
        {
            return Mathf.FloorToInt(Vector3.Distance((Vector3)pos + new Vector3(0.5f, 0.5f, 0), end + new Vector3(0.5f, 0.5f, 0)));
        }

        public List<Vector3> FindPath(float3 startpos, float3 endpos)
        {
            positionToVertex.TryGetValue(new Vector3Int((int)startpos.x, (int)startpos.y, 0), out Vertex start);
            positionToVertex.TryGetValue(new Vector3Int((int)endpos.x, (int)endpos.y, 0), out Vertex end);

            PriorityQueue<Vertex> open = new PriorityQueue<Vertex>();
            List<Vertex> closed = new List<Vertex>();
            start.gValue = 0;
            Dictionary<Vertex, Vertex> parent = new Dictionary<Vertex, Vertex>();

            int lowestH = int.MaxValue;
            Vertex lowestHVertex = null;

            void Insert(Vertex vertex)
            {
                vertex.hValue = Heuristic(vertex.position, endpos);
                if (vertex.hValue < lowestH)
                {
                    lowestH = vertex.hValue;
                    lowestHVertex = vertex;
                }
                open.Enqueue(vertex);
            }
            void UpdateVertex(Vertex s, Vertex sPrime)
            {
                var gOld = sPrime.gValue;
                ComputeCost(s, sPrime);
                if (sPrime.gValue < gOld)
                {
                    if (open.Contains(sPrime))
                    {
                        open.Remove(sPrime);
                    }
                    Insert(sPrime);
                }
            }
            int C(Vertex s, Vertex sPrime)
            {
                return Mathf.FloorToInt(Vector3.Distance((Vector3)s.position + new Vector3(0.5f, 0.5f, 0), (Vector3)sPrime.position + new Vector3(0.5f, 0.5f, 0)));
            }
            void ComputeCost(Vertex s, Vertex sPrime)
            {
                if (LineOfSight(parent[s], sPrime))
                {
                    if (parent[s].gValue + C(parent[s], sPrime) < sPrime.gValue)
                    {
                        parent[sPrime] = parent[s];
                        sPrime.gValue = parent[s].gValue + C(parent[s], sPrime);
                    }
                }
                else
                {
                    if (s.gValue + C(s, sPrime) < sPrime.gValue)
                    {
                        parent[sPrime] = s;
                        sPrime.gValue = s.gValue + C(s, sPrime);
                    }
                }
            }

            parent[start] = start;
            Insert(start);

            while (open.Count() > 0)
            {
                var s = open.Dequeue();
                if (s.Equals(end))
                {
                    return GetPath(parent, end, start, startpos, endpos, true);
                }
                closed.Add(s);
                foreach (var sPrime in s.Neighbors)
                {
                    if (!closed.Contains(sPrime))
                    {
                        if (!open.Contains(sPrime))
                        {
                            sPrime.gValue = int.MaxValue;
                            parent[sPrime] = null;
                        }
                        UpdateVertex(s, sPrime);
                    }
                }
            }

            //No path. Return path from lowest H value
            return GetPath(parent, lowestHVertex, start, startpos, endpos, false);
        }

        private List<Vector3> GetPath(Dictionary<Vertex, Vertex> parent, Vertex end, Vertex start, Vector3 startPos, Vector3 endPos, bool validPath)
        {
            List<Vector3> path = new List<Vector3>();
            Vertex current = end;
            if (validPath)
                path.Add(endPos);
            else
                path.Add(end.position);
            while (!current.Equals(start))
            {
                if (!current.Equals(end))
                    path.Add(current.position + new Vector3(0.5f, 0.5f, 0));
                current = parent[current];
            }
            path.Add(startPos);
            path.Reverse();
            return path;
        }

        private bool LineOfSight(Vertex s, Vertex sPrime)
        {
            Vector3 start = (Vector3)s.position + new Vector3(0.5f, 0.5f, 0);
            Vector3 end = (Vector3)sPrime.position + new Vector3(0.5f, 0.5f, 0);
            Vector3 difference = end - start;
            RaycastHit2D hit = Physics2D.CircleCast(start,
                0.4f,//Radius for enemies
                math.normalize(difference).xy,
                math.length(difference), layerMask);
            return hit.transform == null;
        }



        private void SetupNeighbors()
        {
            //setup neighbors pass
            for (int y = 0; y < tilemapSize.y; y++)
            {
                for (int x = 0; x < tilemapSize.x; x++)
                {
                    Vector3Int pos = tilemapsStart + new Vector3Int(x, y, 0);

                    if (positionToVertex.TryGetValue(pos, out Vertex vertex))
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            for (int i = -1; i <= 1; i++)
                            {
                                if (i == 0 && j == 0)
                                    continue;
                                if (positionToVertex.TryGetValue(pos + new Vector3Int(i, j, 0), out Vertex neighbor))
                                {
                                    if (neighbor.colliderType == ColliderType.None && LineOfSight(vertex, neighbor))
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
                        positionToVertex[pos] = vertex;
                    }
                }
            }
        }

        void Update()
        {

        }
    }

}