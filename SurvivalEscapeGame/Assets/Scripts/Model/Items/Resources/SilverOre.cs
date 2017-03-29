using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilverOre : Item {
    public SilverOre(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["SilverOre"];
        this.Name = thisTextNode["Name"];
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
    }

    public SilverOre(int id, int depthLevel, bool active) : this(id, depthLevel, active, 1) {
    }

    public SilverOre(int id, bool active) : this(id, 0, active) {
    }

    public SilverOre(SilverOre g) : base(g) {

    }
}
