using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StructureData : MonoBehaviour {
    public Dictionary<string, Item> Inventory;
    public List<GameObject> ItemContainer;

   // public static PlayerInput Pi;
    public PlayerData Pd;

    public string Name;

    protected void Awake() {
        Inventory = new Dictionary<string, Item>();
        ItemContainer = new List<GameObject>();
     //   Pi = GameObject.Find("Player").GetComponent<PlayerInput>();
        Pd = GameObject.Find("Player").GetComponent<PlayerData>();
    }
}
