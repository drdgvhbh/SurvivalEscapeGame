using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tent : ActionItem {
    public static Object prefab = Resources.Load("Prefabs/Tent");
    public Tent(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        this.Name = Global.ItemNames[ItemList.Tent];
        this.MaximumQuantity = 1;
        this.Slot = -1;
        this.Icon = Resources.Load<Sprite>("Sprites/Items/Tent");
        this.StaminaCost = 20.0f;
        this.ChannelDuration = 3.0f;
        this.Consumable = true;
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
