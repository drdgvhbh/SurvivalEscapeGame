using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotInput : MonoBehaviour, IDropHandler {
    public int SlotID { get; set; }
    public Item StoredItem { get; set; }
    public bool CraftingSlot { get; set; }
    public bool IsStructureSlot { get; set; }
    public GameObject AssociatedStructure { get; set; }

    public bool IsLocked { get; set; }

    public static PlayerData Pd;

    [SerializeField]
    private GameObject RedX;

    private GameObject ActiveRedX;

    [SerializeField]
    private GameObject ItemObj;


    protected void Awake() {

    } 

    public void OnDrop(PointerEventData eventData) {
        if (PlayerData.Slots[SlotID] == null)
            return;
        ItemInput droppedItem = eventData.pointerDrag.GetComponent<ItemInput>();
        SlotInput otherSlot = droppedItem.GetComponent<ItemInput>().OriginalParent.GetComponent<SlotInput>();
        if (otherSlot == this || IsLocked || otherSlot.IsLocked)
            return;
        Debug.Assert(PlayerData.Slots[SlotID].transform.childCount == 0 || PlayerData.Slots[SlotID].transform.childCount == 1);
        if (!CraftingSlot && !otherSlot.CraftingSlot && !IsStructureSlot && !otherSlot.IsStructureSlot
            || CraftingSlot && otherSlot.CraftingSlot
            || IsStructureSlot && otherSlot.IsStructureSlot) {
            SlotToSlot(droppedItem, otherSlot);
        } else if (CraftingSlot && !otherSlot.CraftingSlot && !otherSlot.IsStructureSlot) {
            SlotCraftTransfer(droppedItem, otherSlot, PlayerData.CraftingInventory, PlayerData.NumCraftingSlots,
                PlayerData.CraftingSlots, PlayerData.CraftingItems, Pd.GetInventory());
        } else if (!CraftingSlot && otherSlot.CraftingSlot && !otherSlot.IsStructureSlot) {
            SlotCraftTransfer(droppedItem, otherSlot, Pd.GetInventory(), PlayerData.NumItemSlots,
                PlayerData.Slots, PlayerData.Items, PlayerData.CraftingInventory);
        } else if (Pd.CurrentTile.Structure.Value != null) {
            Dictionary<String, Item> structureInventory = Pd.CurrentTile.Structure.Value.GetComponent<StructureData>().Inventory;
            List<GameObject> itContainer = Pd.CurrentTile.Structure.Value.GetComponent<StructureData>().ItemContainer;
            if (IsStructureSlot && !otherSlot.IsStructureSlot) {         
                 SlotStructureTransfer(droppedItem, otherSlot, structureInventory, PlayerData.NumStructSlots, PlayerData.StructureSlots, itContainer, Pd.GetInventory());
            } else if (!IsStructureSlot && otherSlot.IsStructureSlot) {
                SlotStructureTransfer(droppedItem, otherSlot, Pd.GetInventory(), PlayerData.NumItemSlots, PlayerData.Slots, PlayerData.Items, structureInventory);
            }
        } 
    }

    protected void SlotCraftTransfer(ItemInput droppedItem, SlotInput otherSlot, Dictionary<string, Item> InventoryAdd, int numSlots,
        List<GameObject> slotsContainer, List<GameObject> itemsContainer, Dictionary<string, Item> InventoryRemove) {
        Item it = otherSlot.StoredItem;
        var type = it.GetType();
        var obj = (Item)Activator.CreateInstance(type, it);
        obj.ActiveContainer = null;
        obj.SetQuantity(1);
        Pd.AddItem(
            obj,
            InventoryAdd,
            numSlots,
            slotsContainer,
            itemsContainer,
            true
            );
        Pd.RemoveItem(it, 1, InventoryRemove);
    }

    protected void SlotStructureTransfer(ItemInput droppedItem, SlotInput otherSlot, Dictionary<string, Item> InventoryAdd, int numSlots,
        List<GameObject> slotsContainer, List<GameObject> itemsContainer, Dictionary<string, Item> InventoryRemove) {
        SlotCraftTransfer(droppedItem, otherSlot, InventoryAdd, numSlots,
            slotsContainer, itemsContainer, InventoryRemove);
        //Granary 
        if (Pd.CurrentTile.Structure.Value.GetComponent<StructureData>() is GranaryData
            && InventoryRemove == Pd.GetInventory()) {
            GranaryData gD = (GranaryData)Pd.CurrentTile.Structure.Value.GetComponent<StructureData>();
            GameObject asdf = itemsContainer.Find(g => g.GetComponent<ItemInput>().Item.GetName().Equals(droppedItem.Item.GetName()));
            gD.StartCoroutine(gD.MutliplyItem(asdf.gameObject.transform.parent.GetComponent<SlotInput>()));
            if (InventoryRemove == Pd.GetInventory()
                    && InventoryAdd[droppedItem.Item.GetName()].GetQuantity() == 1
                    && Pd.CurrentTile.Structure.Value.GetComponent<StructureData>() is GranaryData) {
            }
        }
        //Torch
        if (!(Pd.CurrentTile.Structure.Value.GetComponent<StructureData>() is GranaryData)
                && InventoryRemove == Pd.GetInventory() && droppedItem.Item.GetName().Equals(Global.ItemNames[ItemList.Torch])) {
            Pd.FOWStructures.Add(Pd.CurrentTile.Structure.Value);
            Debug.Log("hello");
        } else if (!(Pd.CurrentTile.Structure.Value.GetComponent<StructureData>() is GranaryData)
                && !(InventoryRemove == Pd.GetInventory()) && droppedItem.Item.GetName().Equals(Global.ItemNames[ItemList.Torch])) {
            Pd.FOWStructures.Remove(Pd.CurrentTile.Structure.Value);
        }
    }

    protected void SlotToSlot(ItemInput droppedItem, SlotInput otherSlot) {
        if (PlayerData.Slots[this.SlotID].transform.childCount == 1) {
            otherSlot.StoredItem = this.StoredItem;
            otherSlot.StoredItem.Slot = otherSlot.SlotID;
            GameObject thisItem = PlayerData.Slots[this.SlotID].GetComponentInChildren<ItemInput>().gameObject;
            thisItem.transform.SetParent(otherSlot.transform);
            thisItem.transform.localPosition = Vector2.zero;
        } else {
            otherSlot.StoredItem.Slot = this.SlotID;
            otherSlot.StoredItem = null;
        }
        droppedItem.transform.SetParent(this.transform);
        droppedItem.transform.localPosition = Vector2.zero;
        this.StoredItem = droppedItem.GetComponent<ItemInput>().Item;
        this.StoredItem.Slot = this.SlotID;
    }

    public bool LockSlot() {
        if (ActiveRedX == null) {
            ActiveRedX = GameObject.Instantiate(RedX);
            ActiveRedX.transform.SetParent(transform, false);
        }
        if (IsLocked)
            return false;
        IsLocked = true;
        //Debug.Assert(ActiveRedX == null, "This slot isn't locked yet but it has a red X.");
        return true;
    }

    public bool UnlockSlot() {
        if (!IsLocked)
            return false;
        //Debug.Assert(ActiveRedX != null, "This slot is locked but it doesnt have a red X.");
        IsLocked = false;
        GameObject.Destroy(ActiveRedX);
        return true;
    }

    public GameObject PopulateSlot(Item it, List<GameObject> itemContainer, Dictionary<String, Item> inventory, int i ) {
        GameObject item = Instantiate(ItemObj);
        itemContainer.Add(item);
        inventory[it.GetName()].ItemObject = item;
        if (this.GetComponent<SlotInput>().IsStructureSlot) {
            item.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
        } else {
            item.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        item.transform.SetParent(this.transform, false);
        item.transform.localPosition = Vector3.zero;

        item.GetComponent<Image>().sprite = inventory[it.GetName()].Icon;
        TMPro.TextMeshProUGUI tmPro = item.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
        tmPro.text = it.GetQuantity().ToString();
        tmPro.alignment = TMPro.TextAlignmentOptions.Right;
        item.GetComponent<ItemInput>().Item = it;
        if (this.GetComponent<SlotInput>().CraftingSlot) {
            it.Slot = i + PlayerData.NumItemSlots;
        } else if (this.GetComponent<SlotInput>().IsStructureSlot) {
            it.Slot = i + PlayerData.NumItemSlots + PlayerData.NumCraftingSlots;
        } else {
            it.Slot = i;
        }
        this.GetComponent<SlotInput>().StoredItem = it;
        if (this.GetComponent<SlotInput>().IsLocked) {
            LockSlot();
        }
        return item;
    }
}

