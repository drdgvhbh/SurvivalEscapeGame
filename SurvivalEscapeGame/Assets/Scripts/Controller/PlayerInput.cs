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

    public Dictionary<string, Action> Actions = new Dictionary<string, Action>() {
    };

    // Use this for initialization
    private void Awake() {
        Actions.Add(Global.ItemNames[ItemList.Shovel], Dig);
        Actions.Add(Global.ItemNames[ItemList.Tent], BuildTent);
    }

    private void Update() {
        // Movement
        if (this.PlayerData == null)
            return;
        this.Movement(this.GetPlayerData().PerformingAction[PlayerActions.Move]);
        Digging();
        ToggleInventory();
        BuildingTent();
    }

    private void Movement(bool exec) {
        if (!exec && !this.GetPlayerData().IsPerformingAction) {
            if (Input.GetAxisRaw("Vertical") > 0) {
                this.NeighbourIndex = (int)Sides.Top;
                this.Move();
            } else if (Input.GetAxisRaw("Vertical") < 0) {
                this.NeighbourIndex = (int)Sides.Bottom;
                this.Move();
            } else if (Input.GetAxisRaw("Horizontal") > 0) {
                this.NeighbourIndex = (int)Sides.Right;
                this.Move();
            } else if (Input.GetAxisRaw("Horizontal") < 0) {
                this.NeighbourIndex = (int)Sides.Left;
                this.Move();
            }
        } else if (exec) {
            float step = this.GetPlayerData().MovementSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, this.Destination, step);
            if (transform.position.Equals(this.Destination)) {
                if (this.GetPlayerData().GetCurrentTile().GetNeighbours()[this.NeighbourIndex] != null) {
                    this.GetPlayerData().SetCurrentTile(this.GetPlayerData().GetCurrentTile().GetNeighbours()[this.NeighbourIndex]);
                }
                this.GetPlayerData().PerformingAction[PlayerActions.Move] = false;
                this.GetPlayerData().IsPerformingAction = false;
                this.GetPlayerData().UpdateTileVisibility();
            }
        }
    }

    protected void Move() {
        if (this.GetPlayerData().GetCurrentTile().GetNeighbours()[this.NeighbourIndex] != null) {
            this.Destination = this.GetPlayerData().GetCurrentTile().GetNeighbours()[this.NeighbourIndex].GetPosition() - Global.SmallOffset;
            this.GetPlayerData().PerformingAction[PlayerActions.Move] = true;
            this.GetPlayerData().IsPerformingAction = true;
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
        if (AItemFcns(PlayerActions.Dig, ItemList.Shovel)) {
            Debug.Log("Digging");
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


