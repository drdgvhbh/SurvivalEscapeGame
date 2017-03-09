using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tent : ActionItem {
    public static List<KeyValuePair<string, int>> CraftingComponents = new List<KeyValuePair<string, int>>() {
    };

    public Object prefab; 
    public Tent(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        prefab = Resources.Load("Prefabs/Tent"); 
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Tent"];
        this.Name = thisTextNode["Name"];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.StaminaCost = thisTextNode["StaminaCost"];
        this.ChannelDuration = thisTextNode["ChannelDuration"];
        this.Consumable = thisTextNode["Consumable"];
        for (int i = 0; i < thisTextNode["Components"].Count; i++) {
            Tent.CraftingComponents.Add(new KeyValuePair<string, int>(
                thisTextNode["Components"][i]["Type"], 
                thisTextNode["Components"][i]["Quantity"])
                );
        }      

    }

    public Tent(int id, int depthLevel, bool active, int quantity) : this(id, depthLevel, active ) {
    }


    public Tent(int id, bool active) : this(id, 0, active) {
    }

    public Tent(Tent t) : base(t) {
    }

    public void BuildTent(PlayerData pd) {
        Tile tile = pd.GetCurrentTile();
        if (!tile.Structures.ContainsKey(ItemList.Tent)){
            pd.Stamina = pd.Stamina - StaminaCost;
            GameObject tent = (GameObject)GameObject.Instantiate(prefab);
            tent.transform.position = tile.GetPosition() - Global.SmallOffset;
            tile.Structures.Add(ItemList.Tent, tent);
            pd.RemoveItem(this, 1, pd.GetInventory());
            pd.GUIText.GetComponent<Text>().text = "Tent built";
        } else {
            pd.GUIText.GetComponent<Text>().text = "This tile already has a tent built on it!";
            Debug.Log("Tile already has a tent built on it!");
        }
    }
}
