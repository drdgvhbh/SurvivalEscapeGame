using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cocoberry : Food {
    public static List<KeyValuePair<string, int>> CraftingComponents = new List<KeyValuePair<string, int>>() {
    };

    public Cocoberry(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Cocoberry"];
        this.Name = thisTextNode["Name"];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.StaminaCost = thisTextNode["StaminaCost"];
        this.ChannelDuration = thisTextNode["ChannelDuration"];
        this.Consumable = thisTextNode["Consumable"];
        this.NourishmentReplenishment = thisTextNode["NourishmentReplenishment"];
        this.HealthReplenishment = thisTextNode["HealthReplenishment"];
        this.NourishmentReplenishment = thisTextNode["NourishmentReplenishment"];
        this.StaminaReplenishment = thisTextNode["StaminaReplenishment"];
        this.HealthReplenishment = thisTextNode["HealthReplenishment"];
        for (int i = 0; i < thisTextNode["Components"].Count; i++) {
            Cocoberry.CraftingComponents.Add(new KeyValuePair<string, int>(
                thisTextNode["Components"][i]["Type"],
                thisTextNode["Components"][i]["Quantity"])
                );
        }
    }

    public Cocoberry(int id, bool active) : this(id, 0, active, 1) {
    }

    public Cocoberry(Cocoberry s) : base(s) {
    }
}
