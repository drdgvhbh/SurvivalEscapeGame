﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tent : BuildingItem {
    public static List<KeyValuePair<string, int>> CraftingComponents = new List<KeyValuePair<string, int>>() {
    };

    public Tent(int id, int depthLevel, bool active) : base(id, depthLevel, active) {
        prefab = Resources.Load("Prefabs/Tent"); 
        var thisTextNode = ItemDatabase.JsonNode["Items"]["Tent"];
        this.Name = thisTextNode["Name"];
        this.MaximumQuantity = thisTextNode["MaximumQuantity"];
        this.Slot = -1;
        this.Icon = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
        this.StaminaCost = thisTextNode["StaminaCost"];
        this.ChannelDuration = thisTextNode["ChannelDuration"];
        this.Consumable = thisTextNode["Consumable"];
        for (int i = 0; i < thisTextNode["Components"].Count; i++) {
            Tent.CraftingComponents.Add(new KeyValuePair<string, int>(
                thisTextNode["Components"][i]["Type"], 
                thisTextNode["Components"][i]["Quantity"])
                );
        }      

    }

    public Tent(int id, int depthLevel, bool active, int quantity) : this(id, depthLevel, active ) {
    }


    public Tent(int id, bool active) : this(id, 0, active) {
    }

    public Tent(Tent t) : base(t) {
        prefab = Resources.Load("Prefabs/Tent");
    }
}
