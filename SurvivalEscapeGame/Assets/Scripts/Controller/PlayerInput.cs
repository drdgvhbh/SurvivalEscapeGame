using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour {
    public Vector3 Destination = new Vector3();
    [SerializeField]
    private PlayerData PlayerData;
    private int NeighbourIndex;
    [SerializeField]
    private GameObject InventoryPanel;

    private Vector3 previousPosition = Vector3.zero;

    [SerializeField]
    private GameObject ChannelingBarMask;

    private float LerpStep = 0.0f;

    public Text GUItext;

    public Dictionary<string, Action> Actions = new Dictionary<string, Action>() {
    };

    // Use this for initialization
    private void Awake() {
        Actions.Add(Global.ItemNames[ItemList.Shovel], Dig);
        Actions.Add(Global.ItemNames[ItemList.Tent], BuildTent);
        Actions.Add(Global.ItemNames[ItemList.Coconut], EatCoconut);
    }

    private void Start() {
        GUItext = GetPlayerData().GUIText.GetComponent<Text>();
    }

    private void Update() {
        // Movement
        if (this.PlayerData == null)
            return;
        this.Movement(this.GetPlayerData().PerformingAction[PlayerActions.Move]);
        Digging();
        ToggleInventory();
        BuildingTent();
        EatingCoconut();
        Attack();
    }

    private void Movement(bool exec) {
        if (!exec && !this.GetPlayerData().IsPerformingAction) {
            if (Input.GetAxisRaw("Vertical") > 0) {
                this.NeighbourIndex = (int)Sides.Top;
                this.Move(GetPlayerData().Direction);
            } else if (Input.GetAxisRaw("Vertical") < 0) {
                this.NeighbourIndex = (int)Sides.Bottom;
                this.Move(GetPlayerData().Direction);
            } else if (Input.GetAxisRaw("Horizontal") > 0) {
                this.NeighbourIndex = (int)Sides.Right;
                this.Move(1);
            } else if (Input.GetAxisRaw("Horizontal") < 0) {
                this.NeighbourIndex = (int)Sides.Left;
                this.Move(-1);
            }
        } else if (exec) {
            Tile enter = this.GetPlayerData().GetCurrentTile().GetNeighbours()[this.NeighbourIndex];
            float step = (this.GetPlayerData().MovementSpeed / enter.MovementCost) * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, this.Destination, step);
            if (transform.position.Equals(this.Destination)) {                                   
                this.GetPlayerData().SetCurrentTile(enter);
                this.GetPlayerData().position = GetPlayerData().CurrentTile.GetPosition() - Global.SmallOffset;
                GetPlayerData().CurrentTile.CurrentGameObject = this.gameObject;
                this.GetPlayerData().PerformingAction[PlayerActions.Move] = false;
                this.GetPlayerData().IsPerformingAction = false;
                this.GetPlayerData().UpdateTileVisibility();
            }
        }
    }

    protected void Move(int direction) {
        Tile enter = this.GetPlayerData().GetCurrentTile().GetNeighbours()[this.NeighbourIndex];
        if (enter != null && enter.CurrentGameObject == null && enter.IsWalkable == true) {
            this.Destination = enter.GetPosition() - Global.SmallOffset;
            this.GetPlayerData().PerformingAction[PlayerActions.Move] = true;
            this.GetPlayerData().IsPerformingAction = true;
            this.GetPlayerData().CurrentTile.CurrentGameObject = null;
            enter.CurrentGameObject = this.gameObject;
        }
        Animator animCtrl = this.gameObject.GetComponent<Animator>();
            
            if (direction == 1) {
            GetPlayerData().Direction = 1;
            if (!(GetPlayerData().IsWeaponEquipped && GetPlayerData().IsShieldEquipped)) {
                animCtrl.ResetTrigger("MoveLeft");
                animCtrl.SetTrigger("MoveRight");
               
            }
        } else if (direction == -1) {
            GetPlayerData().Direction = -1;
            if (!(GetPlayerData().IsWeaponEquipped && GetPlayerData().IsShieldEquipped)) {
                animCtrl.ResetTrigger("MoveRight");
                animCtrl.SetTrigger("MoveLeft");
               
            }
        }
    }

    protected void Attack() {
        if (!Input.GetMouseButtonDown(1)) {
            return;
        }
        if (GetPlayerData().IsPerformingAction || GetPlayerData().Stamina < GetPlayerData().AttackStaminaCost) {
                GUItext.text = "Not enough stamina! Need " + GetPlayerData().AttackStaminaCost + " stamina!!!";
                return;
            }       
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f)) {
            Animator animCtrl = this.gameObject.GetComponent<Animator>();
            Vector2 diff = ray.origin - GetPlayerData().position;
            if (!GetPlayerData().IsWeaponEquipped) {
                if (diff.x > 0) {
                    animCtrl.ResetTrigger("MoveRight");
                    animCtrl.SetTrigger("MoveRight");
                    animCtrl.SetTrigger("AttackNoWeaponRight");
                } else {
                    animCtrl.ResetTrigger("MoveLeft");
                    animCtrl.SetTrigger("MoveLeft");
                    animCtrl.SetTrigger("AttackNoWeaponLeft");
                }
            }
            GetPlayerData().Stamina -= GetPlayerData().AttackStaminaCost;
            Tile[] neigh = GetPlayerData().CurrentTile.GetNeighbours();
            if (Math.Abs(diff.x) > Math.Abs(diff.y)) {
                if (diff.x > 0 && neigh[2].CurrentGameObject != null) {
                    neigh[2].CurrentGameObject.GetComponent<EnemyData>().DamageEnemy(GetPlayerData().Damage);

                } else if (diff.x < 0 && neigh[1].CurrentGameObject != null) {
                    neigh[1].CurrentGameObject.GetComponent<EnemyData>().DamageEnemy(GetPlayerData().Damage);

                }
            } else {
                if (diff.y > 0 && neigh[0].CurrentGameObject != null) {
                    neigh[0].CurrentGameObject.GetComponent<EnemyData>().DamageEnemy(GetPlayerData().Damage);

                } else if (diff.y < 0 && neigh[3].CurrentGameObject != null) {
                    neigh[3].CurrentGameObject.GetComponent<EnemyData>().DamageEnemy(GetPlayerData().Damage);                   
                }
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

    public void BuildTent() {
        if (AItemFcns(PlayerActions.BuildTent, ItemList.Tent)) {
            GUItext.text = "Building tent.";
            Debug.Log("Building Tent");
            ChannelingBarMask.SetActive(true);
            BuildingTent();
        }
    }

    protected void BuildingTent() {
        if (Channeling(ItemList.Tent, PlayerActions.BuildTent) == PlayerActions.BuildTent) {
            Tent ai = (Tent)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Tent]]);
            ai.BuildTent(this.GetPlayerData());
        }
    }

    public void Dig() {
        if (GetPlayerData().CurrentTile.Type == TileType.Mountain) {
            GUItext.text = "Cannnot use a shovel in a mountain!";
            return;
        }
        if (AItemFcns(PlayerActions.Dig, ItemList.Shovel)) {            
            GUItext.text = "Digging... This tile has been dug " + GetPlayerData().CurrentTile.DigCount + " times.";
            ChannelingBarMask.SetActive(true);
            Digging();            
        }
    }

    protected void Digging() {
        if (Channeling(ItemList.Shovel, PlayerActions.Dig) == PlayerActions.Dig) {
            Shovel s = (Shovel)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Shovel]]);
            s.Dig(this.GetPlayerData());
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


