using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : ActionItem {
    public static List<KeyValuePair<string, int>> CraftingComponents = new List<KeyValuePair<string, int>>() {
    };

    public Radar(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Radar"];
        this.Name = thisTextNode["Name"];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.StaminaCost = thisTextNode["StaminaCost"];
        this.ChannelDuration = thisTextNode["ChannelDuration"];
        this.Consumable = thisTextNode["Consumable"];
        for (int i = 0; i < thisTextNode["Components"].Count; i++) {
            Radar.CraftingComponents.Add(new KeyValuePair<string, int>(
                thisTextNode["Components"][i]["Type"],                
                thisTextNode["Components"][i]["Quantity"])
                );
        }
    }

    public Radar(int id, bool active) : this(id, 0, active) {
    }

    public Radar(Radar s) : base(s) {
    }
}
