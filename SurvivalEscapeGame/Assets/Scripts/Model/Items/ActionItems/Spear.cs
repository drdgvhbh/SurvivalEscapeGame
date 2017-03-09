using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : ActionItem {
    public float Damage;
    public Spear(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Spear"];
        this.Name = thisTextNode[Name];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.StaminaCost = thisTextNode["StaminaCost"];
        this.ChannelDuration = thisTextNode["ChannelDuration"];
        this.Consumable = thisTextNode["Consumable"];
        this.Damage = 15.0f;
    }

    public Spear(int id, bool active) : this(id, 0, active) {
    }

    public Spear(Spear s) : base(s) {
    }
}
