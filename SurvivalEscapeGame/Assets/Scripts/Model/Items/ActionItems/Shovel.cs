using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shovel : ActionItem {
    public Shovel(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Shovel"];
        this.Name = thisTextNode["Name"];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.StaminaCost = thisTextNode["StaminaCost"];
        this.ChannelDuration = thisTextNode["ChannelDuration"];
        this.Consumable = thisTextNode["Consumable"];
    }

    public Shovel(int id, bool active) : this(id, 0, active) {
    }

    public Shovel(Shovel s) : base(s) {
    }

    public void Dig(PlayerData pd, PlayerInput pi) {
        pd.CurrentTile.DigCount++;
        Debug.Log("Done Digging");
        GameObject guiTxt = pd.GUIText;
        pd.Stamina = pd.Stamina - StaminaCost;
        Tile tile = pd.GetCurrentTile();
        if (tile.GetTileDepth() == 0) {
            guiTxt.GetComponent<Text>().text = "This tile can be dug no more...";
            return;
        }
        tile.SetTileDepth(tile.GetTileDepth() - 1);
        foreach (Item it in tile.GetItems()) {
            if (it.GetDepthLevel() == tile.GetTileDepth()) {
                tile.RemoveItem(it.GetId());
                if (pd.AddItem(it)) {
                    guiTxt.GetComponent<Text>().text = "Found: " + it.GetName() + "!";
                    Debug.Log("Found: " + it.GetName() + ", Player now has: " + pd.GetInventory()[it.GetName()].GetQuantity() + " " + it.GetName() + "(s).");
                    pi.ItemPickup.Play();
                } else {
                    guiTxt.GetComponent<Text>().text = "Nothing was found.";
                }
            }
        }
    }
}
