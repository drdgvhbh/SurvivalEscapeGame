using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyData : MonoBehaviour {
    public bool IsPerformingAction { get; set; }
    public Tile CurrentTile { get; set; }
    public Vector3 Position { get; set; }
    public bool IsAlive { get; set; }
    private GameObject Player { get; set; }

    private void Awake() {
        IsPerformingAction = false;
        IsAlive = true;
    }

    private void Start() {

    }

    public void Initialize(GameObject player) {
        Debug.Assert(CurrentTile != null);
        Position = CurrentTile.GetPosition();
        this.transform.position = Position - Global.SmallOffset;
        Player = player;
        CalculatePath();
    }

    // Update is called once per frame
    private void Update() {
        //Move();
    }

    private Tile CalculatePath() {
        Tile destination = Player.GetComponent<PlayerData>().CurrentTile;
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();
        PathNode baseNode = new PathNode(CurrentTile, null);
        PathNode MoveToNode = new PathNode(null, null);
        closedList.Add(baseNode);
        Tile[] baseNodeNeighbours = baseNode.GetTile().GetNeighbours();
        closedList = closedList.Union(PathNode.GetTilesAsList(baseNodeNeighbours, baseNode, false, destination, openList)).ToList();
        openList = openList.Union(PathNode.GetTilesAsList(baseNodeNeighbours, baseNode, true, destination, closedList)).ToList();        
        bool IsDestinationFound = false;
        while (IsDestinationFound == false && openList.Count > 0) {
            PathNode lowest = PathNode.GetLowestNode(openList);
            openList.Remove(lowest);
            closedList.Add(lowest);
            closedList = closedList.Union(PathNode.GetTilesAsList(lowest.GetTile().GetNeighbours(), lowest, false, destination, openList)).ToList();
            openList = openList.Union(PathNode.GetTilesAsList(lowest.GetTile().GetNeighbours(), lowest, true, destination, closedList)).ToList();
          
            if (lowest.GetTile() == destination) {
                IsDestinationFound = true;                
            }
            MoveToNode = lowest;            
        }
        while (MoveToNode.GetParent() != null && MoveToNode.GetParent().GetTile() != CurrentTile) {
            MoveToNode = (MoveToNode.GetParent());
            Object prefab = Resources.Load("Prefabs/Tent");
            GameObject tent = (GameObject)GameObject.Instantiate(prefab);
            tent.transform.position = (MoveToNode.GetTile().GetPosition() - Global.SmallOffset);
        }
        return MoveToNode.GetTile();
    }
  
    private void Foo() {

    }

    private void Move() {

    }
}
