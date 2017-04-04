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
    public float ReturnDmg;

    protected new void Awake() {
        base.Awake();
        Name = WallNode["Name"];
        Level = 1;
        ReturnDmg = WallNode["Levels"][Level.ToString()]["ReturnDmg"];
        MaxLevel = WallNode["MaxLevel"];
        NumLocked = WallNode["Levels"][Level.ToString()]["LockedSlots"];
        Health = WallNode["Levels"][Level.ToString()]["Health"];
    }

    protected new void Start() {

    }

    public override void LevelUp() {

        if (Level < MaxLevel) {
            base.LevelUp();
            NumLocked = WallNode["Levels"][Level.ToString()]["LockedSlots"];
            Health = WallNode["Levels"][Level.ToString()]["Health"];
        }
        Debug.Log(Level);
    }
}
