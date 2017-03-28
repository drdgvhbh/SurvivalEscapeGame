using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class DiggingItem : ActionItem {
    public DiggingItem(int id, int depthLevel, bool active) : base(id, depthLevel, active) {

    }

    public DiggingItem(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {

    }

    public DiggingItem(int id, bool active) : base(id, active) {

    }

    public DiggingItem(DiggingItem s) : base(s) {
    }

    public void Dig(PlayerData pd, PlayerInput pi) {
        pd.CurrentTile.NumDigs++;
        Debug.Log("Done Digging");
        GameObject guiTxt = pd.GUIText;
        pd.Stamina = pd.Stamina - StaminaCost;
        Tile tile = pd.GetCurrentTile();
        if (tile.TileDepth == 0) {
            pi.ErrorSound.Play();
            guiTxt.GetComponent<Text>().text = "This tile can be dug no more...";
            return;
        }
        tile.TileDepth = tile.TileDepth - 1;
        List<Item> temp = new List<Item>(tile.Items);
        foreach (Item it in temp) {
            if (it.GetDepthLevel() == tile.TileDepth) {
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
