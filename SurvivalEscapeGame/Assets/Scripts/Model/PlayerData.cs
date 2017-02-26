using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerData : MonoBehaviour {
    public Tile CurrentTile;
    public Vector3 position;
    public bool IsPerformingAction;
    public int NourishmentLevel;
    public float NourishmentStatus;
    public float NourishmentDecayRate;
    public float HealthRegeneration;
    public float MaximumHealth;
    public float Health;
    public float MaximumStamina;
    public float Stamina;
    public float MovementSpeed;
    public bool Alive;
    //private List<Item> Inventory;

    private Dictionary<string, Item> Inventory;

    public Dictionary<PlayerActions, bool> PerformingAction;

    // Use this for initialization
    private void Start() {
        this.NourishmentLevel = 2;
        this.MaximumHealth = NourishmentLevels.BaseMaximumHealth[this.NourishmentLevel];
        this.Health = NourishmentLevels.BaseMaximumHealth[this.NourishmentLevel];
        this.MaximumStamina = NourishmentLevels.BaseMaximumStamina[this.NourishmentLevel];
        this.Stamina = NourishmentLevels.BaseMaximumStamina[this.NourishmentLevel];
        this.MovementSpeed = NourishmentLevels.BaseMovementSpeed[this.NourishmentLevel];
        this.HealthRegeneration = NourishmentLevels.BaseHealthRegeneration[this.NourishmentLevel];
        this.NourishmentStatus = NourishmentLevels.NourishmentThreshold[this.NourishmentLevel] / 2;
        this.NourishmentDecayRate = NourishmentLevels.NourishmentDecayRate[this.NourishmentLevel];
        this.Alive = true;
        this.position = this.GetComponent<Transform>().position;
        this.IsPerformingAction = false;
        this.PerformingAction = new Dictionary<PlayerActions, bool>() {
            {PlayerActions.Move, false },
            {PlayerActions.Dig, false }
        };
        this.Inventory = new Dictionary<string, Item>();
        this.Inventory.Add(Global.ItemNames[ItemList.Shovel], new Shovel(++Item.IdCounter, true));
        this.UpdateTileVisibility();
    }

    // Update is called once per frame
    private void Update() {
        this.Stamina = NourishmentLevels.BaseMaximumStamina[this.NourishmentLevel];
        this.MovementSpeed = NourishmentLevels.BaseMovementSpeed[this.NourishmentLevel];
        this.HealthRegeneration = NourishmentLevels.BaseHealthRegeneration[this.NourishmentLevel];
        this.UpdateHealth();
        this.ApplyNourishmentDecay();
        this.UpdateNourishmentStatus();
        //  Debug.Log(this.NourishmentStatus);        
    }

    public void UpdateTileVisibility() {

        if (this.GetComponentInParent<Model>().IsDay()) {
            this.UpdateTileVisibilityDay();
        } else {
            this.UpdateTileVisibilityNight();
        }
        Model m = this.GetComponentInParent<Model>();
        m.GetGameGrid().SetNormals(m.GetMeshBuilder().Norms.ToList());
    }

    private void UpdateTileVisibilityDay() {
        foreach (Tile t in this.GetCurrentTile().GetExtendedNeighbours(2)) {
            for (int j = 0; j < t.NormIdx.Length; j++) {
                t.Norms[t.NormIdx[j]].Set(0f, 0f, 0f);
                t.SetActive(false);
            }
        }
        foreach (Tile t in this.GetCurrentTile().GetExtendedNeighbours(1)) {
            for (int j = 0; j < t.NormIdx.Length; j++) {
                t.Norms[t.NormIdx[j]].Set(0f, 0f, -1f);
                this.GetCurrentTile().SetActive(true);
            }
        }
    }

    private void UpdateTileVisibilityNight() {
        foreach (Tile t in this.GetCurrentTile().GetExtendedNeighbours(1)) {
            for (int j = 0; j < t.NormIdx.Length; j++) {
                t.Norms[t.NormIdx[j]].Set(0f, 0f, 0f);
                t.SetActive(false);
            }
        }
        this.GetCurrentTile().SetActive(true);
        for (int i = 0; i < this.GetCurrentTile().NormIdx.Length; i++) {
            this.GetCurrentTile().Norms[this.GetCurrentTile().NormIdx[i]].Set(0f, 0f, -1f);
        }
    }

    public void UpdateHealth() {
        if (this.Health < NourishmentLevels.BaseMaximumHealth[this.NourishmentLevel]) {
            this.Health = System.Math.Min(NourishmentLevels.BaseMaximumHealth[this.NourishmentLevel], this.Health + this.HealthRegeneration);
        }
    }

    public void ApplyNourishmentDecay() {
        if (this.NourishmentStatus > 0) {
            this.NourishmentStatus = this.NourishmentStatus - this.NourishmentDecayRate;
        } else if (this.NourishmentLevel == -2) {
            this.NourishmentStatus = 0;
        }
    }

    public void UpdateNourishmentStatus() {
        if (this.NourishmentLevel > -2 && this.NourishmentStatus <= 0) {
            this.NourishmentLevel--;
            this.NourishmentStatus = NourishmentLevels.NourishmentThreshold[this.NourishmentLevel] + this.NourishmentStatus;
        } else if (this.NourishmentLevel < 2 && this.NourishmentStatus > NourishmentLevels.NourishmentThreshold[this.NourishmentLevel + 1]) {
            this.NourishmentLevel++;
            this.NourishmentStatus = NourishmentLevels.NourishmentThreshold[this.NourishmentLevel] - this.NourishmentStatus;
        }
    }

    public void AddItem(Item it) {
        if (!this.GetInventory().ContainsKey(it.GetName())) {
            this.GetInventory().Add(it.GetName(), it);
        } else {
            Item tmp = this.GetInventory()[it.GetName()];
            tmp.SetQuantity(tmp.GetQuantity() + 1);
        }
    }

    public bool InventoryContains(string key) {
        if (this.Inventory.ContainsKey(key)) {
            return true;
        }
        return false;
    }

    public void SetCurrentTile(Tile t) {
        this.CurrentTile = t;
    }

    public Tile GetCurrentTile() {
        return this.CurrentTile;
    }

    public Dictionary<string, Item> GetInventory() {
        return this.Inventory;
    }
}
