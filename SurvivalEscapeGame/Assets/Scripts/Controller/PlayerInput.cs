using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour {
    public Vector3 Destination = new Vector3();
    [SerializeField]
    private PlayerData PlayerData;
    private Tile.Sides NeighbourIndex;
    [SerializeField]
    private GameObject InventoryPanel;

    private Vector3 previousPosition = Vector3.zero;

    [SerializeField]
    private GameObject ChannelingBarMask;

    private float LerpStep = 0.0f;
    private float AttackStep = 0.0f;

    public Text GUItext;

    private AudioSource DigSound;
    private AudioSource WalkSound;
    public AudioSource ErrorSound, RadarFound, RadarNotFound;

    private AudioSource AttackSound;
    public AudioSource ItemPickup;

    private AudioSource Nourish;

    public Tile.Sides Direction;

    private Tile DestinationTile;

    public Dictionary<string, Action> Actions = new Dictionary<string, Action>() {
    };

    public enum PlayerAnimationActions {
        MoveRight,
        MoveLeft,
        MoveUp,
        MoveDown,
        IdleLeft,
        IdleRight,
        DigRight,
        AttackRight,
        AttackLeft,
        DamagedRight,
        DamagedLeft,
    };

    public static Dictionary<PlayerAnimationActions, string> AnimationActions = new Dictionary<PlayerAnimationActions, string>() {
        {PlayerAnimationActions.MoveRight, "MoveRight" },
        {PlayerAnimationActions.MoveLeft, "MoveLeft" },
        {PlayerAnimationActions.MoveUp, "MoveUp" },
        {PlayerAnimationActions.MoveDown, "MoveDown" },
        {PlayerAnimationActions.IdleLeft, "IdleLeft" },
        {PlayerAnimationActions.IdleRight, "IdleRight" },
        {PlayerAnimationActions.DigRight, "DigRight" },
        {PlayerAnimationActions.AttackRight, "AttackRight" },
        {PlayerAnimationActions.AttackLeft, "AttackLeft" },
        {PlayerAnimationActions.DamagedRight, "DamagedRight" },
        {PlayerAnimationActions.DamagedLeft, "DamagedLeft" },
    };


    // Use this for initialization
    private void Awake() {
        Actions.Add(Global.ItemNames[ItemList.Shovel], Dig);
        Actions.Add(Global.ItemNames[ItemList.Tent], BuildTent);
        Actions.Add(Global.ItemNames[ItemList.Coconut], EatCoconut);
        Actions.Add(Global.ItemNames[ItemList.Berry], EatBerry);
        Actions.Add(Global.ItemNames[ItemList.Cocoberry], EatCocoberry);
        Actions.Add(Global.ItemNames[ItemList.Granary], BuildGranary);
        Actions.Add(Global.ItemNames[ItemList.Wall], BuildWall);
        Actions.Add(Global.ItemNames[ItemList.Spear], UseSpear);
        Actions.Add(Global.ItemNames[ItemList.Banana], EatBanana);
        Actions.Add(Global.ItemNames[ItemList.Radar], UseRadar);
        Actions.Add(Global.ItemNames[ItemList.DistressBeacon], UseBeacon);
    }

    private void Start() {
        GUItext = GetPlayerData().GUIText.GetComponent<Text>();
        DigSound = this.GetComponents<AudioSource>()[0];
        WalkSound = this.GetComponents<AudioSource>()[1];
        AttackSound = this.GetComponents<AudioSource>()[2];
        ItemPickup = this.GetComponents<AudioSource>()[3];
        Nourish = this.GetComponents<AudioSource>()[4];
        ErrorSound = this.GetComponents<AudioSource>()[5];
        RadarNotFound = this.GetComponents<AudioSource>()[6];
        RadarFound = this.GetComponents<AudioSource>()[7];
    }

    private void Update() {
        // Movement
        if (this.PlayerData == null)
            return;
        this.Movement();
        Digging();
        ToggleInventory();
        BuildingTent();
        BuildingGranary();
        EatingCoconut();
        EatingBerry();
        EatingCocoberry();
        BuildingWall();
        UsingSpear();
        EatingBanana();
        UsingRadar();
        UsingBeacon();
        if (Input.GetMouseButtonDown(1)) {
            Attack();
        }
        AttackCooldown();
    }

    private void Movement() {
        if (!this.GetPlayerData().IsPerformingAction) {
            WalkSound.loop = true;
            DestinationTile = PlayerData.CurrentTile;
            if (Input.GetAxisRaw("Vertical") > 0) {
                Direction = Tile.Sides.Top;
                if (PlayerData.CurrentTile.Neighbours.ContainsKey(Tile.Sides.Top))
                    DestinationTile = PlayerData.CurrentTile.Neighbours[Tile.Sides.Top];
            } else if (Input.GetAxisRaw("Vertical") < 0) {
                Direction = Tile.Sides.Bottom;
                if (PlayerData.CurrentTile.Neighbours.ContainsKey(Tile.Sides.Bottom))
                    DestinationTile = PlayerData.CurrentTile.Neighbours[Tile.Sides.Bottom];
            } else if (Input.GetAxisRaw("Horizontal") > 0) {
                Direction = Tile.Sides.Right;
                if (PlayerData.CurrentTile.Neighbours.ContainsKey(Tile.Sides.Right))
                    DestinationTile = PlayerData.CurrentTile.Neighbours[Tile.Sides.Right];
            } else if (Input.GetAxisRaw("Horizontal") < 0) {
                Direction = Tile.Sides.Left;
                if (PlayerData.CurrentTile.Neighbours.ContainsKey(Tile.Sides.Left))
                    DestinationTile = PlayerData.CurrentTile.Neighbours[Tile.Sides.Left];
            } else {
                WalkSound.loop = false;
                return;
            }
            if (!WalkSound.isPlaying)
                WalkSound.Play();
            float step = Mathf.Abs(PlayerData.MovementSpeed / PlayerData.CurrentTile.MovementCost) * Time.deltaTime;

            if (DestinationTile != PlayerData.CurrentTile && DestinationTile.CurrentGameObject == this.gameObject) {
                DestinationTile.CurrentGameObject = null;
            }            

            if (DestinationTile.IsWalkable && (DestinationTile.CurrentGameObject == null || DestinationTile.CurrentGameObject == this.gameObject)) {
                transform.position = Vector3.MoveTowards(transform.position, DestinationTile.Position, step);
            } else if (Vector3.Distance(DestinationTile.Position, PlayerData.CurrentTile.Position) > 0.5) {
                transform.position = Vector3.MoveTowards(transform.position, PlayerData.CurrentTile.Position, step);
            }
            PlayerData.DiscoverTiles();
            PlayerData.CalculateCurrentTIle();
        }
    }

    public void Move() {
        Animator animCtrl = this.gameObject.GetComponent<Animator>();
        foreach (KeyValuePair<PlayerAnimationActions, string> e in AnimationActions) {
            animCtrl.ResetTrigger(e.Value);
        }
        if (Direction == Tile.Sides.Right) {
            animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.IdleRight]);
           // animCtrl.ResetTrigger(AnimationActions[PlayerAnimationActions.IdleRight]);
            animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.MoveRight]);
        } else if (Direction == Tile.Sides.Left) {
            animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.IdleLeft]);
            //animCtrl.ResetTrigger(AnimationActions[PlayerAnimationActions.IdleLeft]);
            animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.MoveLeft]);
        } else if (Direction == Tile.Sides.Bottom) {
            animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.IdleRight]);
          //  animCtrl.ResetTrigger(AnimationActions[PlayerAnimationActions.IdleRight]);
            animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.MoveDown]);
        } else if (Direction == Tile.Sides.Top) {
            animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.IdleRight]);
           // animCtrl.ResetTrigger(AnimationActions[PlayerAnimationActions.IdleRight]);
            animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.MoveUp]);
        }
        /* if ((int)Direction != 2) {
             GetPlayerData().Direction = 1;
             if (!(GetPlayerData().IsWeaponEquipped && GetPlayerData().IsShieldEquipped)) {
                 animCtrl.ResetTrigger("MoveLeft");
                 animCtrl.SetTrigger("MoveRight");

             }
         } else if ((int)Direction != 4) {
             GetPlayerData().Direction = -1;
             if (!(GetPlayerData().IsWeaponEquipped && GetPlayerData().IsShieldEquipped)) {
                 animCtrl.ResetTrigger("MoveRight");
                 animCtrl.SetTrigger("MoveLeft");

             }
         }
         */
    }

    protected void Attack() {
        if (GetPlayerData().Stamina < GetPlayerData().AttackStaminaCost) {
            ErrorSound.Play();
            GUItext.text = "Not enough stamina! Need " + GetPlayerData().AttackStaminaCost + " stamina!!!";
            return;
        } else if (GetPlayerData().IsAttackOnCooldown == true) {
            float multi = Mathf.Lerp(0.0f, 1.0f, AttackStep);
            float asdf = AttackStep - (AttackStep * multi);
            GUItext.text = "Attack is on cooldown! " + asdf.ToString("F2") + " seconds remain.";
            return;
        } else if (GetPlayerData().IsPerformingAction) {
            ErrorSound.Play();
            GUItext.text = "Cannot attack while performing another action!";
            return;
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f)) {
            Animator animCtrl = this.gameObject.GetComponent<Animator>();
            Vector2 diff = ray.origin - GetPlayerData().gameObject.transform.position;
            foreach (KeyValuePair<PlayerAnimationActions, string> e in AnimationActions) {
                animCtrl.ResetTrigger(e.Value);
            }
            Dictionary<Tile.Sides, Tile> neigh = GetPlayerData().CurrentTile.Neighbours;
            bool exec = false;
            if (Math.Abs(diff.x) > Math.Abs(diff.y)) {
                if (diff.x > 0 && neigh.ContainsKey(Tile.Sides.Right) && neigh[Tile.Sides.Right].CurrentGameObject != null) {
                    if (this.GetComponent<PlayerInput>().Direction == Tile.Sides.Top) {
                        animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.MoveRight]);
                    }
                    animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.IdleRight]);
                    animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.AttackRight]);
                    neigh[Tile.Sides.Right].CurrentGameObject.GetComponent<EnemyData>().DamageEnemy(GetPlayerData().Damage);
                    this.GetComponent<PlayerInput>().Direction = Tile.Sides.Right;
                    exec = true;

                } else if (diff.x < 0 && neigh.ContainsKey(Tile.Sides.Left) && neigh[Tile.Sides.Left].CurrentGameObject != null) {
                    if (this.GetComponent<PlayerInput>().Direction == Tile.Sides.Top) {
                        animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.MoveRight]);
                    }
                    animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.IdleLeft]);
                    animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.AttackLeft]);
                    neigh[Tile.Sides.Left].CurrentGameObject.GetComponent<EnemyData>().DamageEnemy(GetPlayerData().Damage);
                    this.GetComponent<PlayerInput>().Direction = Tile.Sides.Left;
                    exec = true;
                }
            } else {
                if (diff.y > 0 && neigh.ContainsKey(Tile.Sides.Top) && neigh[Tile.Sides.Top].CurrentGameObject != null) {
                    neigh[Tile.Sides.Top].CurrentGameObject.GetComponent<EnemyData>().DamageEnemy(GetPlayerData().Damage);
                    animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.IdleRight]);
                    animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.MoveUp]);
                    exec = true;
                } else if (diff.y < 0 && neigh.ContainsKey(Tile.Sides.Bottom) && neigh[Tile.Sides.Bottom].CurrentGameObject != null) {
                    neigh[Tile.Sides.Bottom].CurrentGameObject.GetComponent<EnemyData>().DamageEnemy(GetPlayerData().Damage);
                    animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.IdleRight]);
                    animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.MoveDown]);
                    exec = true;
                }
            }
            if (exec) {
                AttackSound.Play();
                GetPlayerData().Stamina -= GetPlayerData().AttackStaminaCost;
                GetPlayerData().IsAttackOnCooldown = true;
            }

        }
    }

    protected void AttackCooldown() {
        if (GetPlayerData().IsAttackOnCooldown) {
            float step = Mathf.Lerp(0.0f, 1.0f, AttackStep);
            AttackStep += Time.deltaTime / GetPlayerData().AttackCooldown;
            if (step >= 1.0f) {
                GetPlayerData().IsAttackOnCooldown = false;
                AttackStep = 0.0f;
            }
        }
    }

    private bool AItemFcns(PlayerActions pa, ItemList il) {
        ActionItem it = (ActionItem)this.GetPlayerData().GetInventory()[Global.ItemNames[il]];
        if (it == null)
            return false;
        bool exec = this.GetPlayerData().PerformingAction[pa];
        if (!exec && !this.GetPlayerData().IsPerformingAction && GetPlayerData().Stamina >= it.StaminaCost) {
            this.GetPlayerData().PerformingAction[pa] = true;
            this.GetPlayerData().IsPerformingAction = true;
            return true;
        }
        return false;
    }

    protected PlayerActions Channeling(ItemList il, PlayerActions pa) {
        if (this.GetPlayerData().InventoryContains(Global.ItemNames[il])
              && this.GetPlayerData().PerformingAction[pa]) {
            ActionItem ai = (ActionItem)(this.GetPlayerData().GetInventory()[Global.ItemNames[il]]);
            float step = Mathf.Lerp(0.0f, 1.0f, LerpStep);
            LerpStep += Time.deltaTime / ai.ChannelDuration;
            if (1.0f - LerpStep >= 0) {
                ChannelingBarMask.transform.GetChild(0).GetComponent<Image>().fillAmount = 1.0f - LerpStep;
                ChannelingBarMask.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Channeling: "
                    + Math.Round((1.0f - LerpStep) * ai.ChannelDuration, 2);
            }
            if (step >= 1.0f) {
                this.GetPlayerData().PerformingAction[pa] = false;
                this.GetPlayerData().IsPerformingAction = false;
                LerpStep = 0.0f;
                ChannelingBarMask.SetActive(false);
                return pa;
            }
        }
        return PlayerActions.NotThisAction;
    }

    public void UseRadar() {
        if (AItemFcns(PlayerActions.UseRadar, ItemList.Radar)) {
            GUItext.text = "Searching for Sacred Items";
            Debug.Log("Searching for Sacred Items");
            ChannelingBarMask.SetActive(true);
            UsingRadar();
        }
    }

    protected void UsingRadar() {
        if (Channeling(ItemList.Radar, PlayerActions.UseRadar) == PlayerActions.UseRadar) {
            Radar ai = (Radar)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Radar]]);
            GetComponent<PlayerData>().Stamina -= ai.StaminaCost;
            if (ai.IsSacredTileInVisionRange(GetComponent<PlayerData>())) {
                GUItext.text = "There is a sacred item near here.";
                RadarFound.Play();
            } else {
                RadarNotFound.Play();
                GUItext.text = "Nothing was found.";
            }
        }
    }

    public void UseBeacon() {
        if (AItemFcns(PlayerActions.UseBeacon, ItemList.DistressBeacon)) {
            GUItext.text = "Constructing Distress Beacon";
            Debug.Log("Constructing Distress Beacon");
            ChannelingBarMask.SetActive(true);
            UsingBeacon();
        }
    }

    protected void UsingBeacon() {
        if (Channeling(ItemList.DistressBeacon, PlayerActions.UseBeacon) == PlayerActions.UseBeacon) {
            DistressBeacon ai = (DistressBeacon)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.DistressBeacon]]);
            ai.BuildStructure(ItemList.DistressBeacon, this.GetPlayerData());
        }
    }

    public void BuildTent() {
        if (AItemFcns(PlayerActions.BuildTent, ItemList.Tent)) {
            GUItext.text = "Building tent.";
            Debug.Log("Building Tent");
            ChannelingBarMask.SetActive(true);
            BuildingTent();
        }
    }

    protected void UseSpear() {
        if (AItemFcns(PlayerActions.UseSpear, ItemList.Spear)) {
            GUItext.text = "Equipping Spear.";
            Debug.Log("Equipping spear");
            ChannelingBarMask.SetActive(true);
            UsingSpear();
        }
    }

    protected void UsingSpear() {
        if (Channeling(ItemList.Spear, PlayerActions.UseSpear) == PlayerActions.UseSpear) {
            PlayerData pd = GetComponent<PlayerData>();
            Spear ai = (Spear)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Spear]]);
            GUItext.text = "Damage increased from (" + pd.Damage +") -> (" + (float)(pd.Damage + ai.Damage) + ")";
            pd.Damage += ai.Damage;
            pd.RemoveItem(ai, 1, pd.GetInventory());
            Debug.Log("Damage increased from (" + pd.Damage + ") -> (" + (float)(pd.Damage + ai.Damage) + ")");
            pd.IsWeaponEquipped = true;
            if (!pd.IsShieldEquipped) {
                GetComponent<Animator>().runtimeAnimatorController = pd.SpearCtrl;
            }
        }
    }

    protected void BuildingTent() {
        if (Channeling(ItemList.Tent, PlayerActions.BuildTent) == PlayerActions.BuildTent) {
            Tent ai = (Tent)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Tent]]);
            Debug.Log(ai.prefab);
            ai.BuildStructure(ItemList.Tent, this.GetPlayerData());
        }
    }

    public void BuildGranary() {
        if (AItemFcns(PlayerActions.BuildGranary, ItemList.Granary)) {
            GUItext.text = "Building Granary.";
            Debug.Log("Building Granary");
            ChannelingBarMask.SetActive(true);
            BuildingGranary();
        }
    }

    protected void BuildingGranary() {
        if (Channeling(ItemList.Granary, PlayerActions.BuildGranary) == PlayerActions.BuildGranary) {
            Granary ai = (Granary)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Granary]]);
            ai.BuildStructure(ItemList.Granary, this.GetPlayerData());
        }
    }

    public void BuildWall() {
        if (AItemFcns(PlayerActions.BuildWall, ItemList.Wall)) {
            GUItext.text = "Building Wall.";
            Debug.Log("Building Wall");
            ChannelingBarMask.SetActive(true);
            BuildingWall();
        }
    }

    protected void BuildingWall() {
        if (Channeling(ItemList.Wall, PlayerActions.BuildWall) == PlayerActions.BuildWall) {
            Wall ai = (Wall)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Wall]]);
            ai.BuildStructure(ItemList.Wall, this.GetPlayerData());
        }
    }


    public void Dig() {
        if (AItemFcns(PlayerActions.Dig, ItemList.Shovel)) {
            GUItext.text = "Digging... This tile has been dug " + GetPlayerData().CurrentTile.NumDigs + " times.";
            ChannelingBarMask.SetActive(true);
            DigSound.Play();
            Digging();
            Animator animCtrl = this.gameObject.GetComponent<Animator>();
            foreach (KeyValuePair<PlayerAnimationActions, string> e in AnimationActions) {
                animCtrl.ResetTrigger(e.Value);
            }
            animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.IdleRight]);
            animCtrl.SetTrigger(AnimationActions[PlayerAnimationActions.DigRight]);
        }
    }

    protected void Digging() {
        if (Channeling(ItemList.Shovel, PlayerActions.Dig) == PlayerActions.Dig) {
            Shovel s = (Shovel)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Shovel]]);
            s.Dig(this.GetPlayerData(), this);
        }
    }

    public void EatCocoberry() {
        if (AItemFcns(PlayerActions.Eat, ItemList.Cocoberry)) {
            GUItext.text = "Consuming Cocoberry.";
            ChannelingBarMask.SetActive(true);
            EatingCocoberry();
        }
    }

    protected void EatingCocoberry() {
        if (Channeling(ItemList.Cocoberry, PlayerActions.Eat) == PlayerActions.Eat) {
            Cocoberry cocoberry = (Cocoberry)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Cocoberry]]);
            cocoberry.Eat(this.GetPlayerData());
            Nourish.Play();
        }
    }

    public void EatBerry() {
        if (AItemFcns(PlayerActions.Eat, ItemList.Berry)) {
            GUItext.text = "Consuming berry.";
            ChannelingBarMask.SetActive(true);
            EatingBerry();
        }
    }

    protected void EatingBerry() {
        if (Channeling(ItemList.Berry, PlayerActions.Eat) == PlayerActions.Eat) {
            Berry berry = (Berry)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Berry]]);
            berry.Eat(this.GetPlayerData());
            Nourish.Play();
        }
    }

    public void EatCoconut() {
        if (AItemFcns(PlayerActions.Eat, ItemList.Coconut)) {
            GUItext.text = "Consuming coconut.";
            ChannelingBarMask.SetActive(true);
            EatingCoconut();
        }
    }

    protected void EatingCoconut() {
        if (Channeling(ItemList.Coconut, PlayerActions.Eat) == PlayerActions.Eat) {
            Coconut coco = (Coconut)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Coconut]]);
            coco.Eat(this.GetPlayerData());
            Nourish.Play();
        }
    }

    public void EatBanana() {
        if (AItemFcns(PlayerActions.Eat, ItemList.Banana)) {
            GUItext.text = "Consuming banana.";
            ChannelingBarMask.SetActive(true);
            EatingCoconut();
        }
    }

    protected void EatingBanana() {
        if (Channeling(ItemList.Banana, PlayerActions.Eat) == PlayerActions.Eat) {
            Banana bana = (Banana)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Banana]]);
            bana.Eat(this.GetPlayerData());
            Nourish.Play();
        }
    }

    public void SetPlayerData(PlayerData pd) {
        this.PlayerData = pd;
    }

    public PlayerData GetPlayerData() {
        return this.PlayerData;
    }

    public void ToggleInventory() {
        if (Input.GetKeyDown(KeyCode.I)) {
            InventoryPanel.SetActive(!InventoryPanel.activeSelf);
        }
    }


}


