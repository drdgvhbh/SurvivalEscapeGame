using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotInput : MonoBehaviour, IDropHandler {
    public int SlotID { get; set; }
    public Item StoredItem { get; set; }

    public void OnDrop(PointerEventData eventData) {
        if (PlayerData.Slots[SlotID] == null )
            return;
        Debug.Assert(PlayerData.Slots[SlotID].transform.childCount == 0 || PlayerData.Slots[SlotID].transform.childCount == 1);

        ItemInput droppedItem = eventData.pointerDrag.GetComponent<ItemInput>();
        SlotInput otherSlot = droppedItem.GetComponent<ItemInput>().OriginalParent.GetComponent<SlotInput>();
        if (otherSlot == this)
            return;
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
}
