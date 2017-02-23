using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Item {
    public Gem(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        this.Name = Global.ItemNames[ItemList.Gem];
        this.Id = id;
        this.DepthLevel = depthLevel;
        this.Active = active;
        this.Quantity = 1;
    }

    public Gem(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        this.Name = Global.ItemNames[ItemList.Gem];
        this.Id = id;
        this.DepthLevel = depthLevel;
        this.Active = active;
        this.Quantity = quantity;
    }
}
