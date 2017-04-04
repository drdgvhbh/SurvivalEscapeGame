using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconData : StructureData {
    private static JSONNode DataNode;
    public static JSONNode BeaconNode {
        get {
            if (DataNode == null) {
                string jsonString = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/StructureData.json");
                DataNode = JSON.Parse(jsonString)["Structures"]["DistressBeacon"];
            }
            return DataNode;
        }
    }

    public static int DistressRadius = 1;


    public Dictionary<SlotInput, int> Ongoing;

    protected new void Awake() {
        base.Awake();
        Name = BeaconNode["Name"];
        Ongoing = new Dictionary<SlotInput, int>();
        Level = 1;
        MaxLevel = BeaconNode["MaxLevel"]; 
        NumLocked = BeaconNode["Levels"][Level.ToString()]["LockedSlots"];
        Health = BeaconNode["Levels"][Level.ToString()]["Health"];
        DistressRadius = BeaconNode["Levels"][Level.ToString()]["DistressRadius"];
    }

    protected new void Start() {
        int idx = Random.Range(0, GameObject.Find("Model").GetComponent<Model>().Mb.NumTiles);
        GameObject.Find("Model").GetComponent<Model>().CreateSavior(idx);
        Debug.Log("savior created");
        
    }

    public override void LevelUp() {
        if (Level < MaxLevel) {
            base.LevelUp();
            NumLocked = BeaconNode["Levels"][Level.ToString()]["LockedSlots"];
            Health = BeaconNode["Levels"][Level.ToString()]["Health"];
            DistressRadius = BeaconNode["Levels"][Level.ToString()]["DistressRadius"];
        }
    }
}
