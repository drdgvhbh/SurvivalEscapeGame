using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GranaryData : StructureData {
    private static JSONNode DataNode;
    public static JSONNode GranaryNode {
        get {
            if (DataNode == null) {
                string jsonString = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/StructureData.json");
                DataNode = JSON.Parse(jsonString)["Structures"]["Granary"];
            }
            return DataNode;
        }
    }

    public static float Interval = 1.0f;
    public static float MultiplyTime = 5.0f;

    public Dictionary<SlotInput, int> Ongoing;

    protected new void Awake() {
        base.Awake();
        Name = GranaryNode["Name"];
        Ongoing = new Dictionary<SlotInput, int>();
        Level = 1;
        NumLocked = GranaryNode["Levels"][Level.ToString()]["LockedSlots"];
        Health = GranaryNode["Levels"][Level.ToString()]["Health"];

    }

    protected new void Start() {

    }

    public IEnumerator MutliplyItem(SlotInput si) {
        float elapsed = 0.0f;
        si.LockSlot();
        if (!Ongoing.ContainsKey(si)) {
            Ongoing.Add(si, 1);
        } else {
            Ongoing[si] += 1;
        }
        while (elapsed < MultiplyTime) {
            elapsed += Interval;
            yield return new WaitForSeconds(Interval);
        }
        Ongoing[si] -= 1;
        IncreaseItemQuantity(si.StoredItem);
        if (Ongoing[si] <= 0) {
            Ongoing.Remove(si);
            si.UnlockSlot();
        }
    }

    public void IncreaseItemQuantity(Item it) {
        it.SetQuantity(it.GetQuantity() + 1);
        foreach (GameObject g in ItemContainer) {
            if (g != null) {
                g.GetComponentInChildren<TextMeshProUGUI>().text = g.GetComponent<ItemInput>().Item.GetQuantity().ToString();
            }
        }

    }
}
