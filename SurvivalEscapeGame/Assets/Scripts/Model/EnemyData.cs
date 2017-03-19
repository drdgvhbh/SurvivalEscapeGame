using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyData : MonoBehaviour {
    public bool IsPerformingAction { get; set; }
    public Tile CurrentTile { get; set; }
    public Vector3 Position { get; set; }
    public bool IsAlive { get; set; }
    private GameObject Player { get; set; }
    public static float MovementSpeed { get; set; }
    public Tile DestinationTile { get; set; }
    public Tile TileTobeMovedTo { get; set; }
    public bool IsReachable { get; set; }
    public static float AttackDamage { get; set; }
    public static float AttackCooldown { get; set; }
    private bool IsAttackOnCooldown { get; set; }
    private float LerpStep { get; set; }
    private bool intialized = false;
    public float Health;
    public static float MaxHealth;
    public int direction;

    private List<GameObject> TempSight = new List<GameObject>();
    [SerializeField]
    private GameObject HealthBarPrefab;

    private GameObject HealthBar;

    private AudioSource AttackSound;
    private AudioSource PainSound;
    private AudioSource DeathSound;

    private static Grid GameGrid;

    private Dictionary<PlayerActions, bool> TypeOfPerformingAction = new Dictionary<PlayerActions, bool>() {
        { PlayerActions.Move, false },
        { PlayerActions.Attack, false }
    };

    private void Awake() {
        IsPerformingAction = false;
        IsAlive = true;
    }

    private void Start() {
        AttackSound = this.GetComponents<AudioSource>()[1];
        DeathSound = this.GetComponents<AudioSource>()[2];
    }

    public void Initialize(GameObject player, Grid gameGrid) {
        Debug.Assert(CurrentTile != null);
        Position = CurrentTile.Position;
        this.transform.position = Position;
        Player = player;
        TileTobeMovedTo = CurrentTile;
        MovementSpeed = 0.97f;
        AttackDamage = 12.0f;
        AttackCooldown = 1.33333f;
        LerpStep = 0.0f;
        IsAttackOnCooldown = false;
        direction = 1;
        MaxHealth = 50.0f;
        Health = MaxHealth;
        intialized = true;
        GameGrid = gameGrid;
        StartCoroutine(Actions(0.25f));

    }

    private void Update() {
        Move();
        Attack();
    }

    private IEnumerator Actions(float waitTime) {
        while (true) {
            if (!intialized )
                yield return new WaitForSeconds(waitTime);
            if (!IsPerformingAction && Player != null) {
                DestinationTile = Player.GetComponent<PlayerData>().CurrentTile;
                int distance = PathNode.GetDistanceToNode(CurrentTile, DestinationTile, GameGrid);
                //Debug.Log(distance);
                if (distance > 5) {
                    yield return new WaitForSeconds(waitTime);
                }


                TileTobeMovedTo = CalculatePath();
                if (TileTobeMovedTo != CurrentTile && TileTobeMovedTo != DestinationTile && (TileTobeMovedTo.CurrentGameObject == null || TileTobeMovedTo.CurrentGameObject == this.gameObject)) {
                    TypeOfPerformingAction[PlayerActions.Move] = true;
                    Vector3 dest = TileTobeMovedTo.Position;
                    Animator animCtrl = this.gameObject.GetComponent<Animator>();
                    if (dest.x > Position.x) {
                        animCtrl.ResetTrigger("GoLeft");
                        animCtrl.SetTrigger("GoRight");
                        direction = 1;
                    } else if (dest.x < Position.x) {
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
                    Player.GetComponent<PlayerData>().DiscoverTiles();
                }
            } else {

            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void DamageEnemy(float damage) {
        float oldHealth = Health;
        Health -= damage;
        if (Health < 0) {
            Player.GetComponent<PlayerInput>().GUItext.text = "Enemy took " + damage + " damage! Enemy destroyed!";
            Player.GetComponent<PlayerData>().AddItem(new Fabric(Item.IdCounter++, 0, true, Random.Range(1, 4)));
            AudioSource.PlayClipAtPoint(DeathSound.clip, CurrentTile.Position, 10.0f);            
            Destroy(this.gameObject);
        } else {
            Player.GetComponent<PlayerInput>().GUItext.text = "Enemy took " + damage + " damage! Enemy Health: " + oldHealth + "->" + Health;
            //PainSound.Play();
        }
    }


    private Tile CalculatePath() {
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

            Object prefab = Resources.Load("Prefabs/Placeholder");
            GameObject tent = (GameObject)GameObject.Instantiate(prefab);
            TempSight.Add(tent);
            tent.transform.position = MoveToNode.GetTile().Position;
            
                     
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

    private void Attack() {
        if (TypeOfPerformingAction[PlayerActions.Attack]) {
            if (IsAttackOnCooldown == false) {
                AttackSound.Play();
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
