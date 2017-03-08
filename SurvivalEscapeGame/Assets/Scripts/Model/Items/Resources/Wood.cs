using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class Wood : Item {
    public Wood(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        this.Name = Global.ItemNames[ItemList.Wood];
        this.Icon = Resources.Load<Sprite>("Sprites/Items/wood");
        this.MaximumQuantity = 5;
        this.Slot = -1;
    }

    public Wood(int id, int depthLevel, bool active) : this(id, depthLevel, active, 1) {
    }

    public Wood(Wood g) : base(g) {

    }
}
