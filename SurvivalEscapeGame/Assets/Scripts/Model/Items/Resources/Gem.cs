using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Item {
    public Gem(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Gem"];
        this.Name = thisTextNode["Name"];
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
    }

    public Gem(int id, int depthLevel, bool active) : this(id, depthLevel, active, 1) {
    }

    public Gem(Gem g) : base(g) {

    }
}
