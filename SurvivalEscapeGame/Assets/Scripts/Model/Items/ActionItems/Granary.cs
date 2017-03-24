using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Granary : BuildingItem {
    public static List<KeyValuePair<string, int>> CraftingComponents = new List<KeyValuePair<string, int>>() {
    };

    public Granary(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        prefab = Resources.Load("Prefabs/Granary");
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Granary"];
        this.Name = thisTextNode["Name"];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.StaminaCost = thisTextNode["StaminaCost"];
        this.ChannelDuration = thisTextNode["ChannelDuration"];
        this.Consumable = thisTextNode["Consumable"];
        for (int i = 0; i < thisTextNode["Components"].Count; i++) {
            Granary.CraftingComponents.Add(new KeyValuePair<string, int>(
                thisTextNode["Components"][i]["Type"],
                thisTextNode["Components"][i]["Quantity"])
                );
        }

    }

    public override void AddData(GameObject obj) {
        obj.AddComponent<GranaryData>();
    }

    public Granary(int id, int depthLevel, bool active, int quantity) : this(id, depthLevel, active) {
    }


    public Granary(int id, bool active) : this(id, 0, active) {
    }

    public Granary(Granary t) : base(t) {
    }
}
