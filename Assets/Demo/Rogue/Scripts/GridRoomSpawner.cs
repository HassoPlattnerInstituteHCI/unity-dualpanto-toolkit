using System.Collections;
using System.Collections.Generic;
using System.EnterpriseServices;
using DualPantoToolkit;
using JetBrains.Annotations;
using UnityEngine;

public class GridRoomSpawner : MonoBehaviour
{
    public class RoomData
    {
        public int id;
        public List<Vector2Int> tiles = new List<Vector2Int>();
        public List<Vector2Int> perimeter = new List<Vector2Int>();

        public RoomData(int id)
        {
            this.id = id;
        }
    }
    public class UnionFind<T>
    {
        Dictionary<T, T> parent = new Dictionary<T, T>();

        public void MakeSet(T x)
        {
            parent[x] = x;
        }

        public T Find(T x)
        {
            if (!EqualityComparer<T>.Default.Equals(parent[x], x))
                parent[x] = Find(parent[x]);
            return parent[x];
        }

        public void Union(T a, T b)
        {
            var ra = Find(a);
            var rb = Find(b);
            if (!ra.Equals(rb))
                parent[rb] = ra;
        }
    }



    public GameObject plane;      // The Plane GameObject in the scene
    public float cellSize = 0.2f;

    public GameObject roomPrefab;

    public GameObject map;

    [HideInInspector] public int gridWidth;
    [HideInInspector] public int gridHeight;


    public int maxRoomSize = 10;
    private int minRoomSize = 5;



    public int[,] grid;

    void Awake()
    {
        
    }

    public void Create(){
        CalculateGridFromPlane();
        grid = new int[gridWidth, gridHeight];
        CreateRooms();
        CreateCorridor();
        map.AddComponent<PantoCompoundCollider>();
    }

    void CalculateGridFromPlane()
    {
        if (plane == null)
        {
            Debug.LogError("Plane not assigned to GridManager.");
            return;
        }

        // Unity's Plane primitive is 10x10 units in size by default
        Vector3 worldScale = plane.transform.localScale;
        float totalWidth = 10f * worldScale.x;
        float totalHeight = 10f * worldScale.z;

        gridWidth = Mathf.FloorToInt(totalWidth / cellSize);
        gridHeight = Mathf.FloorToInt(totalHeight / cellSize);
    }

    public bool IsCellFree(int x, int y)
    {
        return x >= 0 && x < gridWidth &&
               y >= 0 && y < gridHeight &&
               (grid[x, y] == 0);
    }

    public bool CanPlaceRoom(int x, int y, int width, int height)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                if (!IsCellFree(i, j)) return false;
            }
        }
        return true;
    }

    public void OccupyCells(int x, int y, int width, int height, int id)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                grid[i, j] = id;
            }
        }
    }

    public void CreateRooms()
    {
        int maxRooms = Random.Range(4, 6);
        var roomsId = new List<int>();

        int maxTry = 10;
        for (int i = 1; i < maxRooms; i++)
        {
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);

            var randomPos = GetRandomGridCoordiantes();

            int countTry = 0;
            while (!CanPlaceRoom(randomPos.x, randomPos.y, roomWidth, roomHeight) && countTry <= maxTry)
            {
                countTry++;
                randomPos = GetRandomGridCoordiantes();
            }
            if (countTry <= maxTry)
            {
                OccupyCells(randomPos.x, randomPos.y, roomWidth, roomHeight, i);
                roomsId.Add(i);
                AddRoomToMap(randomPos.x, randomPos.y, roomWidth, roomHeight);
            }
        }
    }

    public void CreateCorridor(List<int> roomIds)
    {

    }

    public void AddRoomToMap(int x, int y, int roomWidth, int roomHeight)
    {
        var pos = GetWorldPosition(x, y);

        // Mittelpunkt des ganzen Raums (mehrere Zellen) treffen
        pos += new Vector3((roomWidth - 1) * cellSize * 0.5f, 0f,
                           (roomHeight - 1) * cellSize * 0.5f);

        var room = Instantiate(roomPrefab, pos, Quaternion.identity, map.transform);

        float worldWidth = roomWidth * cellSize;
        float worldDepth = roomHeight * cellSize;

        // 3D-Objekt: über localScale skalieren
        room.transform.localScale = new Vector3(worldWidth,
                                                room.transform.localScale.y,
                                                worldDepth);

        // Falls 2D/SpriteRenderer im Prefab (Draw Mode Tiled/Sliced):
        // var sr = room.GetComponent<SpriteRenderer>();
        // if (sr != null) sr.size = new Vector2(worldWidth, worldDepth);
    }

    (int x, int y) GetRandomGridCoordiantes()
    {
        return (Random.Range(0, gridWidth), Random.Range(0, gridHeight));
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        // Align cell centers to the middle of the plane
        float originX = plane.transform.position.x - (gridWidth * cellSize) / 2f + cellSize / 2f;
        float originZ = plane.transform.position.z - (gridHeight * cellSize) / 2f + cellSize / 2f;

        return new Vector3(originX + x * cellSize, 0, originZ + y * cellSize);
    }

    //ChatGPT krams
    public List<Vector2Int> BFS(Vector2Int start, Vector2Int goal, HashSet<Vector2Int> blocked)
    {
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        q.Enqueue(start);
        visited.Add(start);

        Vector2Int[] dirs = {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1)
    };

        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            if (cur == goal)
            {
                // reconstruct
                List<Vector2Int> path = new List<Vector2Int>();
                var t = goal;
                while (t != start)
                {
                    path.Add(t);
                    t = parent[t];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            foreach (var d in dirs)
            {
                var n = cur + d;

                if (n.x < 0 || n.x >= gridWidth || n.y < 0 || n.y >= gridHeight)
                    continue;

                if (blocked.Contains(n) && n != goal)
                    continue;

                if (!visited.Contains(n))
                {
                    visited.Add(n);
                    parent[n] = cur;
                    q.Enqueue(n);
                }
            }
        }

        return null;
    }
    public List<RoomData> ExtractRooms()
    {
        Dictionary<int, RoomData> dict = new Dictionary<int, RoomData>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int id = grid[x, y];
                if (id > 0)
                {
                    if (!dict.ContainsKey(id))
                        dict[id] = new RoomData(id);

                    dict[id].tiles.Add(new Vector2Int(x, y));
                }
            }
        }

        // Perimeter berechnen
        foreach (var r in dict.Values)
        {
            foreach (var tile in r.tiles)
            {
                bool edge = false;

                foreach (var d in new Vector2Int[]{
                new Vector2Int(1,0),
                new Vector2Int(-1,0),
                new Vector2Int(0,1),
                new Vector2Int(0,-1)})
                {
                    Vector2Int n = tile + d;

                    if (n.x < 0 || n.x >= gridWidth || n.y < 0 || n.y >= gridHeight)
                    {
                        edge = true;
                        break;
                    }

                    if (grid[n.x, n.y] != r.id)
                    {
                        edge = true;
                        break;
                    }
                }

                if (edge)
                    r.perimeter.Add(tile);
            }
        }

        return new List<RoomData>(dict.Values);
    }
    public void CreateCorridor()
    {
        List<RoomData> rooms = ExtractRooms();

        UnionFind<int> uf = new UnionFind<int>();
        foreach (var r in rooms)
            uf.MakeSet(r.id);

        // Verbotene Tiles
        HashSet<Vector2Int> blocked = new HashSet<Vector2Int>();
        foreach (var r in rooms)
            foreach (var t in r.tiles)
                blocked.Add(t);

        // Raum-Paare nach Nähe sortieren
        List<(RoomData A, RoomData B, int dist)> pairs = new List<(RoomData, RoomData, int)>();

        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                int minDist = int.MaxValue;

                foreach (var a in rooms[i].perimeter)
                    foreach (var b in rooms[j].perimeter)
                    {
                        int d = Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
                        if (d < minDist) minDist = d;
                    }

                pairs.Add((rooms[i], rooms[j], minDist));
            }
        }

        pairs.Sort((a, b) => a.dist.CompareTo(b.dist));

        // Verbindungen erstellen
        foreach (var (A, B, _) in pairs)
        {
            if (uf.Find(A.id) == uf.Find(B.id))
                continue;

            List<Vector2Int> best = null;
            int bestLen = int.MaxValue;

            foreach (var from in A.perimeter)
                foreach (var to in B.perimeter)
                {
                    var p = BFS(from, to, blocked);
                    if (p != null && p.Count < bestLen)
                    {
                        best = p;
                        bestLen = p.Count;
                    }
                }

            if (best != null)
            {
                // blockieren
                foreach (var t in best)
                    blocked.Add(t);

                // sichtbar machen
                DrawCorridor(best);

                // Union
                uf.Union(A.id, B.id);
            }
        }
    }
    public void DrawCorridor(List<Vector2Int> path)
    {
        float scaleFactor = 1f;
        foreach (var p in path)
        {
            Vector3 pos = GetWorldPosition(p.x, p.y);
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = pos;
            cube.transform.localScale = new Vector3(cellSize*scaleFactor, 0.1f, cellSize*scaleFactor);
            cube.transform.parent = map.transform;
            cube.name = "CorridorTile";
        }
    }



}
