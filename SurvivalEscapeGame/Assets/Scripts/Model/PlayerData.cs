using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour {
    public Tile CurrentTile;
    public Vector3 position;
    public bool IsPerformingAction;
    //private List<Item> Inventory;

    private Dictionary<string, Item> Inventory;

    public Dictionary<PlayerActions, bool> PerformingAction;

    // Use this for initialization
    private void Start () {
        this.position = this.GetComponent<Transform>().position;
        this.IsPerformingAction = false;
        this.PerformingAction = new Dictionary<PlayerActions, bool>() {
            {PlayerActions.Move, false },
            {PlayerActions.Dig, false }
        };
        this.Inventory = new Dictionary<string, Item>();
        this.Inventory.Add(Global.ItemNames[ItemList.Shovel], new Shovel(++Item.IdCounter, true));
	}
	
	// Update is called once per frame
	private void Update () {
		
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
