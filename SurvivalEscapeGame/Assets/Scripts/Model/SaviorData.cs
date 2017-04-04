using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaviorData : MonoBehaviour {
    public bool IsPerformingAction { get; set; }
    public Tile CurrentTile { get; set; }
    public Vector3 Position { get; set; }
    public bool IsAlive { get; set; }
    private GameObject Player { get; set; }
    public static float MovementSpeed { get; set; }
    public Tile DestinationTile { get; set; }
    public Tile TileTobeMovedTo { get; set; }
    public bool IsReachable { get; set; }
    private float LerpStep { get; set; }
    private bool intialized = false;
    public int direction;

    private List<GameObject> TempSight = new List<GameObject>();
    private static Grid GameGrid;

    private Dictionary<PlayerActions, bool> TypeOfPerformingAction = new Dictionary<PlayerActions, bool>() {
        { PlayerActions.Move, false },
    };

    private void Awake() {
        IsPerformingAction = false;
        IsAlive = true;
    }

    private void Start() {
    }

    public void Initialize(GameObject player, Grid gameGrid) {
        Debug.Assert(CurrentTile != null);
        Position = CurrentTile.Position;
        this.transform.position = Position;
        Player = player;
        TileTobeMovedTo = CurrentTile;
        MovementSpeed = 3.0f;
        LerpStep = 0.0f;
        direction = 1;
        intialized = true;
        GameGrid = gameGrid;
        StartCoroutine(Actions(0.25f));

    }

    private void Update() {
        Move();
    }

    public static class RandomEnum {
        private static System.Random _Random = new System.Random(Environment.TickCount);

        public static T Of<T>() {
            if (!typeof(T).IsEnum)
                throw new InvalidOperationException("Must use Enum type");

            Array enumValues = Enum.GetValues(typeof(T));
            return (T)enumValues.GetValue(_Random.Next(enumValues.Length));
        }
    }

    private void DoThings() {
        if (!intialized) {
            return;
        }
        if (!IsPerformingAction && Player != null) {
            DestinationTile = Player.GetComponent<PlayerData>().CurrentTile;
            int shortestDistance = PathNode.GetDistanceToNode(CurrentTile, DestinationTile, GameGrid);

            foreach (GameObject go in Player.GetComponent<PlayerData>().AllStructures) {
                int distance = PathNode.GetDistanceToNode(CurrentTile, go.GetComponent<StructureData>().CurrentTile, GameGrid);
                if (distance < shortestDistance && go.GetComponent<BeaconData>() != null) {
                      shortestDistance = distance;
                    DestinationTile = go.GetComponent<StructureData>().CurrentTile;
                }
            }
            Tile realDestination = DestinationTile;
            if (shortestDistance > BeaconData.DistressRadius) {
                if (CurrentTile.Neighbours.Count > 0) {
                    Tile.Sides side;
                    do {
                        side = RandomEnum.Of<Tile.Sides>();
                    } while (!CurrentTile.Neighbours.ContainsKey(side));
                    DestinationTile = CurrentTile.Neighbours[side];
                    if (DestinationTile.Neighbours.Count > 0) {
                        do {
                            do {
                                side = RandomEnum.Of<Tile.Sides>();
                            } while (!DestinationTile.Neighbours.ContainsKey(side));
                            DestinationTile = DestinationTile.Neighbours[side];
                        } while (DestinationTile == CurrentTile);
                    }
                } else {
                    return;
                }
            }
            TileTobeMovedTo = CalculatePath();
            if (TileTobeMovedTo != CurrentTile) {
                TypeOfPerformingAction[PlayerActions.Move] = true;
                Vector3 dest = TileTobeMovedTo.Position;
               // Animator animCtrl = this.gameObject.GetComponent<Animator>();
                if (dest.x > Position.x) {
                    direction = 1;
                } else if (dest.x < Position.x) {
                    direction = -1;
                }
                IsPerformingAction = true;
                CurrentTile.Savior = null;
                TileTobeMovedTo.Savior = this.gameObject;
            }
        }
    }

    private IEnumerator Actions(float waitTime) {
        while (true) {
            DoThings();
            yield return new WaitForSeconds(waitTime);
        }
    }

    private Tile CalculatePath() {
        Tile t = CurrentTile;
        int currentRow = CurrentTile.Index / GameGrid.NumColumns;
        int currentColumn = CurrentTile.Index % GameGrid.NumColumns;
        int destRow = DestinationTile.Index / GameGrid.NumColumns;
        int destColumn = DestinationTile.Index % GameGrid.NumColumns;
        int diffX = destColumn - currentColumn;
        int diffY = destRow - currentRow;
        if (diffX != 0) {
            if (diffX > 0) {
                t = CurrentTile.Neighbours[Tile.Sides.Right];
            } else if (diffX < 0) {
                t = CurrentTile.Neighbours[Tile.Sides.Left];
            }
        } else if (diffY != 0) {
            if (diffY > 0) {
                t = CurrentTile.Neighbours[Tile.Sides.Bottom];
            } else if (diffY < 0) {
                t = CurrentTile.Neighbours[Tile.Sides.Top];
            }
        }
        return t;
    }

   /* private Tile CalculatePath() {
        foreach (GameObject g in TempSight) {
            GameObject.Destroy(g);
        }
        TempSight.Clear();
        Tile destination = DestinationTile;
        bool oldWalkable = destination.IsWalkable;
        if (destination.IsWalkable == false) {
            destination.IsWalkable = true;
        }
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();
        PathNode baseNode = new PathNode(CurrentTile, null);
        PathNode MoveToNode = new PathNode(null, null);
        closedList.Add(baseNode);
        Dictionary<Tile.Sides, Tile> baseNodeNeighbours = baseNode.GetTile().Neighbours;
        closedList = closedList.Union(PathNode.GetTilesAsList(baseNodeNeighbours, baseNode, false, destination, openList, GameGrid)).ToList();
        openList = openList.Union(PathNode.GetTilesAsList(baseNodeNeighbours, baseNode, true, destination, closedList, GameGrid)).ToList();
        bool IsDestinationFound = false;
        while (IsDestinationFound == false && openList.Count > 0) {
            PathNode lowest = PathNode.GetLowestNode(openList);
            openList.Remove(lowest);
            closedList.Add(lowest);
            closedList = closedList.Union(PathNode.GetTilesAsList(lowest.GetTile().Neighbours, lowest, false, destination, openList, GameGrid)).ToList();
            openList = openList.Union(PathNode.GetTilesAsList(lowest.GetTile().Neighbours, lowest, true, destination, closedList, GameGrid)).ToList();

            if (lowest.GetTile() == destination) {
                IsDestinationFound = true;
            }
            MoveToNode = lowest;
        }
        while (MoveToNode.GetParent() != null && MoveToNode.GetParent().GetTile() != CurrentTile) {
            //Debug.DrawLine(MoveToNode.GetTile().Position, MoveToNode.GetParent().GetTile().Position, Color.yellow);
            MoveToNode = (MoveToNode.GetParent());

            /*Object prefab = Resources.Load("Prefabs/Placeholder");
            GameObject tent = (GameObject)GameObject.Instantiate(prefab);
            TempSight.Add(tent);
            tent.transform.position = MoveToNode.GetTile().Position;
            */
/*        }
        if (openList.Count < 0) {
            IsReachable = false;
            GameObject.Destroy(this.gameObject);

        } else {
            IsReachable = true;
        }
        if (oldWalkable == false) {
            destination.IsWalkable = oldWalkable;
        }
        return MoveToNode.GetTile();
    }*/

    private void Move() {
        if (TypeOfPerformingAction[PlayerActions.Move]) {
            float step = Mathf.Abs(MovementSpeed / TileTobeMovedTo.MovementCost) * Time.deltaTime;
            Vector3 moveTo = TileTobeMovedTo.Position;
            transform.position = Vector3.MoveTowards(transform.position, moveTo, step);
            if (transform.position.Equals(moveTo)) {
                CurrentTile = TileTobeMovedTo;
                Position = CurrentTile.Position;
                TypeOfPerformingAction[PlayerActions.Move] = false;
                IsPerformingAction = false;
                Player.GetComponent<PlayerData>().DiscoverTiles();
            }
        }
    }
}
