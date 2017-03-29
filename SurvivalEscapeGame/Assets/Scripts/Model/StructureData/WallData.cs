using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WallData : StructureData {
    private static JSONNode DataNode;
    public static JSONNode WallNode {
        get {
            if (DataNode == null) {
                string jsonString = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/StructureData.json");
                DataNode = JSON.Parse(jsonString)["Structures"]["Wall"];
            }
            return DataNode;
        }
    }

    public Dictionary<SlotInput, int> Ongoing;

    protected new void Awake() {
        base.Awake();
        Name = WallNode["Name"];
        Level = 1;
        NumLocked = WallNode["Levels"][Level.ToString()]["LockedSlots"];
        Health = WallNode["Levels"][Level.ToString()]["Health"];
    }

    protected new void Start() {

    }
}
