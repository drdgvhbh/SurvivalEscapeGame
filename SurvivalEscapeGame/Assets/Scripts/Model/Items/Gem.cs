using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Item {
    public Gem(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        this.Name = Global.ItemNames[ItemList.Gem];
        this.Icon = Resources.Load<Sprite>("Sprites/Items/blue_gem_1");
        this.MaximumQuantity = int.MaxValue;
        this.Slot = -1;
    }

    public Gem(int id, int depthLevel, bool active) : this(id, depthLevel, active, 1) {
    }
}
