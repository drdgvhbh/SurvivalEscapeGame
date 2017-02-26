using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : ActionItem {
    public Shovel(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        this.Name = Global.ItemNames[ItemList.Shovel];
        this.Id = id;
        this.DepthLevel = depthLevel;
        this.Active = active;
        this.Quantity = 1;
        this.HungerCost = 0.1f;
        this.ThirstCost = 0.4f;
    }

    public Shovel(int id, bool active) : base(id, active) {
        this.Name = Global.ItemNames[ItemList.Shovel];
        this.Id = id;
        this.DepthLevel = 0;
        this.Active = active;
        this.Quantity = 1;
        this.HungerCost = 0.1f;
        this.ThirstCost = 0.4f;
    }

    public void Dig(PlayerData pd) {
        Debug.Log("Digging");
        Tile tile = pd.GetCurrentTile();
        if (tile.GetTileDepth() == 0) {
            Debug.Log("This tile can be dug no more.");
            return;
        }
        tile.SetTileDepth(tile.GetTileDepth() - 1);
        foreach (Item it in tile.GetItems()) {
            if (it.GetDepthLevel() == tile.GetTileDepth()) {
                tile.RemoveItem(it.GetId());
                pd.AddItem(it);
                Debug.Log("Found: " + it.GetName() + ", Player now has: " + pd.GetInventory()[it.GetName()].GetQuantity() + " " + it.GetName() + "(s).");
            }
        }       
    }
}
