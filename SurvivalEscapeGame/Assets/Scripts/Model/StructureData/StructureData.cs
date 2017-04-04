using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StructureData : MonoBehaviour {
    public Dictionary<string, Item> Inventory;
    public List<GameObject> ItemContainer;

    public Tile CurrentTile;

   // public static PlayerInput Pi;
    public PlayerData Pd;

    public string Name;
    public float Health;
    public bool Active;

    protected int NumLocked;
    protected int Level;
    protected int MaxLevel;

    protected void Awake() {
        Inventory = new Dictionary<string, Item>();
        ItemContainer = new List<GameObject>();
     //   Pi = GameObject.Find("Player").GetComponent<PlayerInput>();
        Pd = GameObject.Find("Player").GetComponent<PlayerData>();
        Active = true;
    }

    protected void Start() {

    }

    public void UpdateOnOpen() {
        for (int i = PlayerData.StructureSlots.Count - 1; i > PlayerData.StructureSlots.Count - (NumLocked + 1); i--) {
            PlayerData.StructureSlots[i].GetComponent<SlotInput>().UnlockSlot();
            PlayerData.StructureSlots[i].GetComponent<SlotInput>().LockSlot();

        }
    }

    public void DamageStructure(float damage) {
        Health -= damage;
        if (Health < 0) {
            CurrentTile.Structure = new KeyValuePair<ItemList, GameObject>();
            Pd.GetComponent<PlayerData>().AllStructures.Remove(this.gameObject);
            GameObject.Destroy(this.gameObject);
        }
    }

    public virtual void LevelUp() {
        Level++;
    }
}
