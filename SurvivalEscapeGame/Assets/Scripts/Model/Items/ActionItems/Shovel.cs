using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : ActionItem {
    public Shovel(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        this.Name = Global.ItemNames[ItemList.Shovel];
        this.HungerCost = 0.1f;
        this.ThirstCost = 0.4f;
        this.MaximumQuantity = 1;
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>("Sprites/Items/ToolsSprites")[1];
    }

    public Shovel(int id, bool active) : this(id, 0, active) {
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
                if (pd.AddItem(it)) {
                    Debug.Log("Found: " + it.GetName() + ", Player now has: " + pd.GetInventory()[it.GetName()].GetQuantity() + " " + it.GetName() + "(s).");
                }
            }
        }
    }
}
