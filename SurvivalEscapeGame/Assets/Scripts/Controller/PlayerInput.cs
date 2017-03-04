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
    private void Start() {
        Actions.Add(Global.ItemNames[ItemList.Shovel], Dig);
    }

    private void Update() {
        // Movement
        if (this.PlayerData == null)
            return;
        this.Movement(this.GetPlayerData().PerformingAction[PlayerActions.Move]);
        Digging();
        ToggleInventory();
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

    public void Dig() {
        bool exec = this.GetPlayerData().PerformingAction[PlayerActions.Dig];
        Shovel s = (Shovel)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Shovel]]);
        if (!exec && !this.GetPlayerData().IsPerformingAction && GetPlayerData().Stamina >= s.StaminaCost) {
            this.GetPlayerData().PerformingAction[PlayerActions.Dig] = true;
            this.GetPlayerData().IsPerformingAction = true;
            Debug.Log("Digging");
            ChannelingBarMask.SetActive(true);
            Digging();
        }
    }

    protected void Digging() {
        if (this.GetPlayerData().InventoryContains(Global.ItemNames[ItemList.Shovel]) 
                && this.GetPlayerData().PerformingAction[PlayerActions.Dig]) {
            Shovel s = (Shovel)(this.GetPlayerData().GetInventory()[Global.ItemNames[ItemList.Shovel]]);
            float step = Mathf.Lerp(0.0f, 1.0f, LerpStep);
            LerpStep += Time.deltaTime / s.ChannelDuration;
            if (1.0f - LerpStep >= 0) {
                ChannelingBarMask.transform.GetChild(0).GetComponent<Image>().fillAmount = 1.0f - LerpStep;
                ChannelingBarMask.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Channeling: " 
                    + Math.Round((1.0f - LerpStep) * s.ChannelDuration, 2);
            }
            if (step >= 1.0f) {
                s.Dig(this.GetPlayerData());
                this.GetPlayerData().PerformingAction[PlayerActions.Dig] = false;
                this.GetPlayerData().IsPerformingAction = false;
                LerpStep = 0.0f;
                ChannelingBarMask.SetActive(false);
            }
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


