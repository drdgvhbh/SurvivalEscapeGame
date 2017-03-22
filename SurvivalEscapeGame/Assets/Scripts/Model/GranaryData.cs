using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranaryData : MonoBehaviour {
    public static int NumGranSlots = 8;

    public Dictionary<string, Item> Inventory;
    public List<GameObject> GranSlots;

    [SerializeField]
    private GameObject Panel;

    [SerializeField]
    private GameObject InventorySlot;

    public void Awake() {
        Inventory = new Dictionary<string, Item>();
        GranSlots = new List<GameObject>();
        for (int i = 0; i < GranaryData.NumGranSlots; i++) {
            GameObject Is = Instantiate(InventorySlot);
            PlayerData.Slots.Add(Is);
            int idx = i + PlayerData.NumItemSlots + PlayerData.NumCraftingSlots;
            SlotInput sI = PlayerData.Slots[idx].AddComponent<SlotInput>();
            sI.SlotID = idx;
            sI.CraftingSlot = false;
            sI.IsStructureSlot = true;
            GranSlots.Add(Is);
            GranSlots[i].transform.SetParent(Panel.transform, false);
        }
    }
}
