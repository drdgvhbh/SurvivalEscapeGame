using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TentData : StructureData {
    private static JSONNode DataNode;
    public static JSONNode TentNode {
        get {
            if (DataNode == null) {
                string jsonString = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/StructureData.json");
                DataNode = JSON.Parse(jsonString)["Structures"]["Tent"];
            }
            return DataNode;
        }
    }

    public static float Interval = 1.0f;
    public static float MultiplyTime = 5.0f;

    public Dictionary<SlotInput, int> Ongoing;

    protected new void Awake() {
        base.Awake();
        Name = TentNode["Name"];
        Ongoing = new Dictionary<SlotInput, int>();
        Level = 1;
        NumLocked = TentNode["Levels"][Level.ToString()]["LockedSlots"];
        Health = TentNode["Levels"][Level.ToString()]["Health"];
    }

    protected new void Start() {

    }
}
