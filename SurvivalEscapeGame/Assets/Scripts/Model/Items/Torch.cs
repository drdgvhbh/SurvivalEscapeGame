using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torch : Item {
    public static List<KeyValuePair<string, int>> CraftingComponents = new List<KeyValuePair<string, int>>() {
    };

    public Torch(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Torch"];
        this.Name = thisTextNode["Name"];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        for (int i = 0; i < thisTextNode["Components"].Count; i++) {
            Torch.CraftingComponents.Add(new KeyValuePair<string, int>(
                thisTextNode["Components"][i]["Type"],
                thisTextNode["Components"][i]["Quantity"])
                );
        }
    }

    public Torch(int id, bool active) : this(id, 0, active, 1) {
    }

    public Torch(Torch g) : base(g) {

    }
}
