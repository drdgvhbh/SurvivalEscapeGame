using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyData : MonoBehaviour {
    public bool IsPerformingAction { get; set; }
    public Tile CurrentTile { get; set; }
    public Vector3 Position { get; set; }
    public bool IsAlive { get; set; }
    private GameObject Player { get; set; }
    private float MovementSpeed { get; set; }
    public Tile DestinationTile { get; set; }
    public Tile TileTobeMovedTo { get; set; }
    public bool IsReachable { get; set; }
    public float AttackDamage { get; set; }
    public float AttackCooldown { get; set; }
    private bool IsAttackOnCooldown { get; set; }
    private float LerpStep { get; set; }
    private bool intialized = false;
    public int direction;

    private List<GameObject> TempSight = new List<GameObject>();

    private Dictionary<PlayerActions, bool> TypeOfPerformingAction = new Dictionary<PlayerActions, bool>() {
        { PlayerActions.Move, false },
        { PlayerActions.Attack, false }
    };

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
        TileTobeMovedTo = CurrentTile;
        MovementSpeed = 1.0f;
        AttackDamage = 15.0f;
        AttackCooldown = 1.33333f;
        LerpStep = 0.0f;
        IsAttackOnCooldown = false;
        direction = 1;
        intialized = true;        
    }

    // Update is called once per frame
    private void Update() {
        if (!intialized || Player.gameObject == null)
            return;
        if (!IsPerformingAction) {
            /*foreach (GameObject g in TempSight) {
                Destroy(g);
            }
            TempSight.Clear();    
            */        
            DestinationTile = Player.GetComponent<PlayerData>().CurrentTile;
            int distance = PathNode.GetDistanceToNode(CurrentTile, DestinationTile);
            if (distance > 5) {               
                return;
            } 

                
            TileTobeMovedTo = CalculatePath();
            if (TileTobeMovedTo != CurrentTile && TileTobeMovedTo != DestinationTile && TileTobeMovedTo.CurrentGameObject == null) {
                TypeOfPerformingAction[PlayerActions.Move] = true;
                Vector3 dest = TileTobeMovedTo.GetPosition() - Global.SmallOffset;
                Animator animCtrl = this.gameObject.GetComponent<Animator>();
                if ( dest.x > Position.x) {
                    animCtrl.ResetTrigger("GoLeft");
                    animCtrl.SetTrigger("GoRight");
                    direction = 1;
                } else if (dest.x < Position.x ) {
                    animCtrl.ResetTrigger("GoRight");
                    animCtrl.SetTrigger("GoLeft");
                    direction = -1;
                }
                IsPerformingAction = true;
                CurrentTile.CurrentGameObject = null;
                TileTobeMovedTo.CurrentGameObject = this.gameObject;
            } else if (CurrentTile.IsAdjacent(DestinationTile)) {
               TypeOfPerformingAction[PlayerActions.Attack] = true;
                IsPerformingAction = true;
            }   
        } else {            
            Move();
            Attack();
        }   
    }

    private Tile CalculatePath() {
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
            /*Object prefab = Resources.Load("Prefabs/Tent");
            GameObject tent = (GameObject)GameObject.Instantiate(prefab);
            TempSight.Add(tent);
            tent.transform.position = (MoveToNode.GetTile().GetPosition() - Global.SmallOffset);
            */            
        }
        if (openList.Count < 0) {
            IsReachable = false;
        } else {
            IsReachable = true;
        }
        if (oldWalkable == false) {
            destination.IsWalkable = oldWalkable;
        }
        return MoveToNode.GetTile();
    }

    private void Move() {
        if (TypeOfPerformingAction[PlayerActions.Move]) {
            float step = (MovementSpeed / TileTobeMovedTo.MovementCost) * Time.deltaTime;
            Vector3 moveTo = TileTobeMovedTo.GetPosition() - Global.SmallOffset;
            transform.position = Vector3.MoveTowards(transform.position, moveTo, step);
            if (transform.position.Equals(moveTo)) {                
                CurrentTile = TileTobeMovedTo;
                Position = CurrentTile.GetPosition();
                TypeOfPerformingAction[PlayerActions.Move] = false;
                IsPerformingAction = false;
                Player.GetComponent<PlayerData>().UpdateTileVisibility();
            }
        }
    }

    private void Attack() {
        if (TypeOfPerformingAction[PlayerActions.Attack]) {
            if (IsAttackOnCooldown == false) {
                Player.GetComponent<PlayerData>().DamagePlayer(AttackDamage);
                Animator animCtrl = this.gameObject.GetComponent<Animator>();
                if (direction == 1) {
                    animCtrl.SetTrigger("GoRight");
                    animCtrl.SetTrigger("AttackRight");
                } else {
                    animCtrl.SetTrigger("GoLeft");
                    animCtrl.SetTrigger("AttackLeft");
                    direction = -1;
                }
                IsAttackOnCooldown = true;
            }
            float step = Mathf.Lerp(0.0f, 1.0f, LerpStep);
            LerpStep += Time.deltaTime / AttackCooldown;
            if (step >= 1.0f) {
                TypeOfPerformingAction[PlayerActions.Attack] = false;
                IsPerformingAction = false;
                IsAttackOnCooldown = false; 
                LerpStep = 0.0f;
                

            }
        }
    }
}
