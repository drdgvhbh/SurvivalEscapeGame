using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : DiggingItem {
    public static List<KeyValuePair<string, int>> CraftingComponents = new List<KeyValuePair<string, int>>() {
        { new KeyValuePair<string, int>(Global.ItemNames[ItemList.Wood], 1) },
        { new KeyValuePair<string, int>(Global.ItemNames[ItemList.Stone], 1) },
    };

    public Pickaxe(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        this.Name = Global.ItemNames[ItemList.Pickaxe];
        this.MaximumQuantity = 1;
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>("Sprites/Items/farmTools")[11];
        this.StaminaCost = 20.0f;
        this.ChannelDuration = 2.5f;
        this.Consumable = false;
    }

    public Pickaxe(int id, bool active) : this(id, 0, active) {
    }

    public Pickaxe(Pickaxe s) : base(s) {
    }
}
