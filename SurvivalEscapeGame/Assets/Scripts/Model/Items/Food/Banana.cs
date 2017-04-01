using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : Food {
    public Banana(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Banana"];
        this.Name = thisTextNode["Name"];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.StaminaCost = thisTextNode["StaminaCost"];
        this.ChannelDuration = thisTextNode["ChannelDuration"];
        this.Consumable = thisTextNode["Consumable"];
        this.NourishmentReplenishment = thisTextNode["NourishmentReplenishment"];
        this.StaminaReplenishment = thisTextNode["StaminaReplenishment"];
        this.HealthReplenishment = thisTextNode["HealthReplenishment"];
    }

    public Banana(int id, bool active) : this(id, 0, active, 1) {
    }

    public Banana(Banana s) : base(s) {
    }
}
