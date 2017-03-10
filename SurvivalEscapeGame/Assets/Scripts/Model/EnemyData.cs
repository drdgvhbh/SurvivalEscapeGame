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

    public void Initialize(GameObject player) {
        Debug.Assert(CurrentTile != null);
        Position = CurrentTile.GetPosition();
        this.transform.position = Position - Global.SmallOffset;
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
        /*
        HealthBar = GameObject.Instantiate(HealthBarPrefab);
        HealthBar.transform.SetParent(GameObject.Find("Canvas").transform);
        RectTransform CanvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();   
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(this.gameObject.transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        //now you can set the position of the ui element
        HealthBar.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
        */

    }

    // Update is called once per frame
    private void Update() {
        /*
        RectTransform CanvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(this.gameObject.transform.position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));
        */

        //now you can set the position of the ui element
       // HealthBar.GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
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

    public void DamageEnemy(float damage) {
        float oldHealth = Health;
        Health -= damage;
        if (Health < 0) {
            Player.GetComponent<PlayerInput>().GUItext.text = "Enemy took " + damage + " damage! Enemy destroyed!";
            Player.GetComponent<PlayerData>().AddItem(new Fabric(Item.IdCounter++, 0, true, Random.Range(1, 4)));
            AudioSource.PlayClipAtPoint(DeathSound.clip, CurrentTile.GetPosition(), 10.0f);            
            Destroy(this.gameObject);
        } else {
            Player.GetComponent<PlayerInput>().GUItext.text = "Enemy took " + damage + " damage! Enemy Health: " + oldHealth + "->" + Health;
            //PainSound.Play();
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
