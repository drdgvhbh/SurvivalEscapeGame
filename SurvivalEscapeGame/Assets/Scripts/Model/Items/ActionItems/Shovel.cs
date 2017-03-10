using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shovel : DiggingItem {
    public Shovel(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Shovel"];
        this.Name = thisTextNode["Name"];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.StaminaCost = thisTextNode["StaminaCost"];
        this.ChannelDuration = thisTextNode["ChannelDuration"];
        this.Consumable = thisTextNode["Consumable"];
    }

    public Shovel(int id, bool active) : this(id, 0, active) {
    }

    public Shovel(Shovel s) : base(s) {
    }  
}
