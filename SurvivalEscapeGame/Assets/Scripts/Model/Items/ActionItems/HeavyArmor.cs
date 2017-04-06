using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyArmour : ActionItem {
    public static List<KeyValuePair<string, int>> CraftingComponents = new List<KeyValuePair<string, int>>() {
    };

    public float HealthBonus;
    public HeavyArmour(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["HeavyArmour"];
        this.Name = thisTextNode["Name"];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.StaminaCost = thisTextNode["StaminaCost"];
        this.ChannelDuration = thisTextNode["ChannelDuration"];
        this.Consumable = thisTextNode["Consumable"];
        this.HealthBonus = thisTextNode["HealthBonus"];
        for (int i = 0; i < thisTextNode["Components"].Count; i++) {
            HeavyArmour.CraftingComponents.Add(new KeyValuePair<string, int>(
                thisTextNode["Components"][i]["Type"],
                thisTextNode["Components"][i]["Quantity"])
                );
        }
    }

    public HeavyArmour(int id, bool active) : this(id, 0, active) {
    }

    public HeavyArmour(HeavyArmour s) : base(s) {
    }
}
