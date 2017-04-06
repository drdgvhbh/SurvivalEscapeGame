using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBananaCocosalad : Food {
    public static List<KeyValuePair<string, int>> CraftingComponents = new List<KeyValuePair<string, int>>() {
    };

    public BerryBananaCocosalad(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["BerryBananaCocosalad"];
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
            BerryBananaCocosalad.CraftingComponents.Add(new KeyValuePair<string, int>(
                thisTextNode["Components"][i]["Type"],
                thisTextNode["Components"][i]["Quantity"])
                );
        }
    }

    public BerryBananaCocosalad(int id, bool active) : this(id, 0, active, 1) {
    }

    public BerryBananaCocosalad(BerryBananaCocosalad s) : base(s) {
    }
}
