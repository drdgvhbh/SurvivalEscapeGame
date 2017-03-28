using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : Item {
    public Stick(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Stick"];
        this.Name = thisTextNode["Name"];
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
    }

    public Stick(int id, int depthLevel, bool active) : this(id, depthLevel, active, 1) {
    }

    public Stick(int id, bool active) : this(id, 0, active, 1) {
    }

    public Stick(Stick g) : base(g) {

    }
}
