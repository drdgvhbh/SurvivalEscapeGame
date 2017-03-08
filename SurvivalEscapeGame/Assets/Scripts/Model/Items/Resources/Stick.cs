using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : Item {
    public Stick(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        this.Name = Global.ItemNames[ItemList.Stick];
        this.Icon = Resources.Load<Sprite>("Sprites/Items/placeholder");
        this.MaximumQuantity = 5;
        this.Slot = -1;
    }

    public Stick(int id, int depthLevel, bool active) : this(id, depthLevel, active, 1) {
    }

    public Stick(Stick g) : base(g) {

    }
}
