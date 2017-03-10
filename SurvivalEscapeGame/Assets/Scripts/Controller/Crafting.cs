using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Crafting : MonoBehaviour, IPointerDownHandler {
    [SerializeField]
    private GameObject CraftingButton;

    [SerializeField]
    private GameObject Player;
    private PlayerData Pd;

    private void Start() {
        
    }

    public static Dictionary<ItemList, Item[]> Types = new Dictionary<ItemList, Item[]>() {
            {ItemList.Pickaxe, new Pickaxe[0] },
            {ItemList.Tent, new Tent[0] },
            {ItemList.Radar, new Radar[0] },
            {ItemList.Cocoberry, new Cocoberry[0] }
    };

    public static Dictionary<ItemList, List<KeyValuePair<string, int>>> CraftableItems = new Dictionary<ItemList, List<KeyValuePair<string, int>>> {
        {ItemList.Pickaxe, Pickaxe.CraftingComponents },
        {ItemList.Tent, Tent.CraftingComponents },
        {ItemList.Radar, Radar.CraftingComponents },
        {ItemList.Cocoberry, Cocoberry.CraftingComponents }
    };

    public void OnPointerDown(PointerEventData eventData) {
        if (Pd == null)
            Pd = Player.GetComponent<PlayerData>();
        ItemList itemToBeCrafted = ItemList.Placeholder;        
        Dictionary<string, Item> CraftInventory = PlayerData.CraftingInventory;
        //List of Craftable Items
        foreach (KeyValuePair<ItemList, List<KeyValuePair<string, int>>> Craftable in CraftableItems) {
            bool canCraft = true;
            //List of each component
            List<Item> ItemsToBeRemoved = new List<Item>();
            foreach (KeyValuePair<string, int> components in Craftable.Value) {
                if (!(CraftInventory.ContainsKey(components.Key) && CraftInventory[components.Key].GetQuantity() >= components.Value)) {
                    canCraft = false;
                } else {
                    ItemsToBeRemoved.Add(CraftInventory[components.Key]);
                }
            }
            if (canCraft) {
                this.GetComponent<AudioSource>().Play();
                itemToBeCrafted = Craftable.Key;
                int oldQuant = 0;
                foreach (Item it in ItemsToBeRemoved) {
                    oldQuant = it.GetQuantity();
                    Pd.RemoveItem(it, it.GetQuantity(), PlayerData.CraftingInventory);
                }
                var type = Types[itemToBeCrafted].GetType().GetElementType();
                var obj = (Item)Activator.CreateInstance(type, ++Item.IdCounter, true);
                obj.ActiveContainer = null;
                obj.SetQuantity(1);
                if (!(Pd.AddItem(obj, Pd.GetInventory(), PlayerData.NumItemSlots, PlayerData.Slots, PlayerData.Items))) {
                    foreach (Item it in ItemsToBeRemoved) {
                        it.SetQuantity(oldQuant);
                        Pd.AddItem(it, PlayerData.CraftingInventory, PlayerData.NumCraftingSlots, PlayerData.CraftingSlots, PlayerData.CraftingItems);
                       
                    }
                }
                
                break;
            }
        }
    }
}
