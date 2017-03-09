using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class Wood : Item {
    public Wood(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Wood"];
        this.Name = thisTextNode["Name"];
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
    }

    public Wood(int id, int depthLevel, bool active) : this(id, depthLevel, active, 1) {
    }

    public Wood(Wood g) : base(g) {

    }
}
