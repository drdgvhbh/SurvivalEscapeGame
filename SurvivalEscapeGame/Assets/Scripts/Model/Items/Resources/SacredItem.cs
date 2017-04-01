using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacredItem : Item {
    public SacredItem(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["SacredItem"];
        this.Name = thisTextNode["Name"];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
    }

    public SacredItem(int id, bool active) : this(id, 0, active, 1) {
    }

    public SacredItem(SacredItem s) : base(s) {
    }
}
