using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fabric : Item {
    public Fabric(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Fabric"];
        this.Name = thisTextNode["Name"];
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
    }

    public Fabric(int id, int depthLevel, bool active) : this(id, depthLevel, active, 1) {
    }

    public Fabric(Fabric g) : base(g) {

    }
}
