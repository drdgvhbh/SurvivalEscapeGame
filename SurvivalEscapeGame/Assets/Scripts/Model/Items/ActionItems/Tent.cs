using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tent : ActionItem {
    public Tent(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        this.Name = Global.ItemNames[ItemList.Shovel];
        this.MaximumQuantity = 1;
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>("Sprites/Items/ToolsSprites")[1];
        this.StaminaCost = 10.0f;
        this.ChannelDuration = 1.5f;
        this.Consumable = false;
    }
}
