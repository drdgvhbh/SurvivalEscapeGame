using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charcoal : Item {
    public Charcoal(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Charcoal"];
        this.Name = thisTextNode["Name"];
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
    }

    public Charcoal(int id, int depthLevel, bool active) : this(id, depthLevel, active, 1) {
    }

    public Charcoal(int id, bool active) : this(id, 0, active, 1) {
    }

    public Charcoal(Charcoal g) : base(g) {

    }
}
