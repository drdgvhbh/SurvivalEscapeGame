using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BuildingItem : ActionItem {
    public Object prefab;
    public BuildingItem(int id, int depthLevel, bool active) : base(id, depthLevel, active) {

    }

    public BuildingItem(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {

    }

    public BuildingItem(int id, bool active) : base(id, active) {

    }

    public BuildingItem(BuildingItem ai) : base(ai) {
        this.StaminaCost = ai.StaminaCost;
        this.ChannelDuration = ai.ChannelDuration;
        this.Consumable = ai.Consumable;
    }

    public void BuildStructure(ItemList it, PlayerData pd) {
        Tile tile = pd.GetCurrentTile();
        if (tile.Structure.Value == null) {
            pd.Stamina = pd.Stamina - StaminaCost;
            GameObject structure = (GameObject)GameObject.Instantiate(prefab);
            structure.transform.position = tile.Position;
            tile.Structure = new KeyValuePair<ItemList, GameObject>(it, structure);
            AddData(structure);
            structure.GetComponent<StructureData>().CurrentTile = tile;
            pd.AllStructures.Add(structure);
            pd.RemoveItem(this, 1, pd.GetInventory());
            pd.GUIText.GetComponent<Text>().text = it + " built.";
        } else {
            pd.GUIText.GetComponent<Text>().text = "This tile already has a structure built on it!";
            Debug.Log("This tile already has a structure built on it!");
        }
    }

    public virtual void AddData(GameObject obj) {

    }
}