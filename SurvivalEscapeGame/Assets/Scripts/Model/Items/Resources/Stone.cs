using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Item {
    public Stone(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        this.Name = Global.ItemNames[ItemList.Stone];
        this.Icon = Resources.Load<Sprite>("Sprites/Items/stone");
        this.MaximumQuantity = 5;
        this.Slot = -1;
    }

    public Stone(int id, int depthLevel, bool active) : this(id, depthLevel, active, 1) {
    }

    public Stone(Stone g) : base(g) {

    }
}
