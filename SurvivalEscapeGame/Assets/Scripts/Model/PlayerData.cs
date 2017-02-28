using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour {
    public Tile CurrentTile;
    public Vector3 position;
    public bool IsPerformingAction;
    public int NourishmentLevel;
    public float NourishmentStatus;
    public float NourishmentDecayRate;
    public float HealthRegeneration;
    public float MaximumHealth;
    public float Health;
    public float MaximumStamina;
    public float Stamina;
    public float MovementSpeed;
    public bool Alive;
    //private List<Item> Inventory;

    private Dictionary<string, Item> Inventory;
    private float MaximumNourishmentStatus;

    public Dictionary<PlayerActions, bool> PerformingAction;

    public GameObject HealthBar;
    public GameObject NourishmentBar;
    public GameObject StaminaBar;

    private const int NumItemSlots = 16;

    [SerializeField]
    private GameObject InventoryPanel;
    [SerializeField]
    private GameObject SlotPanel;
    [SerializeField]
    private GameObject InventorySlot;
    [SerializeField]
    private GameObject InventoryItem;

    public List<GameObject> Slots = new List<GameObject>();
    public List<GameObject> Items = new List<GameObject>();

    // Use this for initialization
    private void Start() {
    }

    public void LateStart() {
        this.NourishmentLevel = 0;
        this.MaximumHealth = NourishmentLevels.BaseMaximumHealth[this.NourishmentLevel];
        this.Health = NourishmentLevels.BaseMaximumHealth[this.NourishmentLevel];
        this.MaximumStamina = NourishmentLevels.BaseMaximumStamina[this.NourishmentLevel];
        this.Stamina = NourishmentLevels.BaseMaximumStamina[this.NourishmentLevel];
        this.MovementSpeed = NourishmentLevels.BaseMovementSpeed[this.NourishmentLevel];
        this.HealthRegeneration = NourishmentLevels.BaseHealthRegeneration[this.NourishmentLevel];
        this.NourishmentStatus = NourishmentLevels.NourishmentThreshold[this.NourishmentLevel] / 2;
        this.NourishmentDecayRate = NourishmentLevels.NourishmentDecayRate[this.NourishmentLevel];
        this.position = this.GetComponent<Transform>().position;
        this.Alive = true;
        this.IsPerformingAction = false;
        this.PerformingAction = new Dictionary<PlayerActions, bool>() {
            {PlayerActions.Move, false },
            {PlayerActions.Dig, false }
        };
        this.Inventory = new Dictionary<string, Item>();
        this.Inventory.Add(Global.ItemNames[ItemList.Shovel], new Shovel(++Item.IdCounter, true));
        this.UpdateTileVisibility();
        this.HealthBar.GetComponent<Image>().fillAmount = this.Health / this.MaximumHealth;
        this.MaximumNourishmentStatus = 0f;
        for (int i = -2; i <= 2; i++) {
            this.MaximumNourishmentStatus += NourishmentLevels.NourishmentThreshold[i];
        }
        for (int i = 0; i < PlayerData.NumItemSlots; i++) {
            this.Slots.Add(Instantiate(InventorySlot));
            this.Slots[i].transform.SetParent(this.SlotPanel.transform);
        }
    }

    // Update is called once per frame
    private void Update() {
        this.Stamina = NourishmentLevels.BaseMaximumStamina[this.NourishmentLevel];
        this.MovementSpeed = NourishmentLevels.BaseMovementSpeed[this.NourishmentLevel];
        this.HealthRegeneration = NourishmentLevels.BaseHealthRegeneration[this.NourishmentLevel];
        this.UpdateHealth();
        this.ApplyNourishmentDecay();
        this.UpdateNourishmentStatus();
        this.UpdateStamina();
        //  Debug.Log(this.NourishmentStatus);        
    }

    public void UpdateTileVisibility() {

        if (this.GetComponentInParent<Model>().IsDay()) {
            this.UpdateTileVisibilityDay();
        } else {
            this.UpdateTileVisibilityNight();
        }
        Model m = this.GetComponentInParent<Model>();
        m.GetGameGrid().SetNormals(m.GetMeshBuilder().Norms.ToList());
    }

    private void UpdateTileVisibilityDay() {
        foreach (Tile t in this.GetCurrentTile().GetExtendedNeighbours(3)) {
            for (int j = 0; j < t.NormIdx.Length; j++) {
                t.Norms[t.NormIdx[j]].Set(0f, 0f, 0f);
                t.SetActive(false);
            }
        }
        foreach (Tile t in this.GetCurrentTile().GetExtendedNeighbours(2)) {
            for (int j = 0; j < t.NormIdx.Length; j++) {
                t.Norms[t.NormIdx[j]].Set(0f, 0f, -1f);
                this.GetCurrentTile().SetActive(true);
            }
        }
    }

    private void UpdateTileVisibilityNight() {
        foreach (Tile t in this.GetCurrentTile().GetExtendedNeighbours(2)) {
            for (int j = 0; j < t.NormIdx.Length; j++) {
                t.Norms[t.NormIdx[j]].Set(0f, 0f, 0f);
                t.SetActive(false);
            }
        }
        foreach (Tile t in this.GetCurrentTile().GetExtendedNeighbours(1)) {
            for (int j = 0; j < t.NormIdx.Length; j++) {
                t.Norms[t.NormIdx[j]].Set(0f, 0f, -1f);
                this.GetCurrentTile().SetActive(true);
            }
        }
    }

    public void UpdateStamina() {
        this.StaminaBar.GetComponent<Image>().fillAmount = this.Stamina / this.MaximumStamina;
    }

    public void UpdateHealth() {
        if (this.Health < NourishmentLevels.BaseMaximumHealth[this.NourishmentLevel]) {
            this.Health = System.Math.Min(NourishmentLevels.BaseMaximumHealth[this.NourishmentLevel], this.Health + this.HealthRegeneration);
        }
        this.HealthBar.GetComponent<Image>().fillAmount = this.Health / this.MaximumHealth;
    }

    public void ApplyNourishmentDecay() {
        if (this.NourishmentStatus > 0) {
            this.NourishmentStatus = this.NourishmentStatus - this.NourishmentDecayRate;
        } else if (this.NourishmentLevel == -2) {
            this.NourishmentStatus = 0;
        }
    }

    public void UpdateNourishmentStatus() {
        if (this.NourishmentLevel > -2 && this.NourishmentStatus <= 0) {
            this.NourishmentLevel--;
            this.NourishmentStatus = NourishmentLevels.NourishmentThreshold[this.NourishmentLevel] + this.NourishmentStatus;
        } else if (this.NourishmentLevel < 2 && this.NourishmentStatus > NourishmentLevels.NourishmentThreshold[this.NourishmentLevel + 1]) {
            this.NourishmentLevel++;
            this.NourishmentStatus = NourishmentLevels.NourishmentThreshold[this.NourishmentLevel] - this.NourishmentStatus;
        }
        float NBarOffset = 0f;
        for (int i = -2; i < this.NourishmentLevel; i++) {
            NBarOffset += NourishmentLevels.NourishmentThreshold[i];
        }
        this.NourishmentBar.GetComponent<Image>().fillAmount = (NBarOffset + this.NourishmentStatus) / this.MaximumNourishmentStatus;
    }

    public void AddItem(Item it) {
        if (this.GetInventory().Count < PlayerData.NumItemSlots)
            return;
        if (!this.GetInventory().ContainsKey(it.GetName())) {
            this.GetInventory().Add(it.GetName(), it);
        } else {
            Item tmp = this.GetInventory()[it.GetName()];
            tmp.SetQuantity(tmp.GetQuantity() + 1);
        }
        /*  for (int i = 0; i < PlayerData.NumItemSlots; i++) {
              if (this.Slots[i].transform.childCount == 0) {
                  GameObject item = Instantiate(this.InventoryItem);
                  Items.Add(item);
                  this.GetInventory()[it.GetName()].ItemObject = item;
                  item.transform.SetParent(this.Slots[i].transform);
                  item.transform.position = Vector3.zero;
                  break;
              }

          }*/



    }

    public bool InventoryContains(string key) {
        if (this.Inventory.ContainsKey(key)) {
            return true;
        }
        return false;
    }

    public void SetCurrentTile(Tile t) {
        this.CurrentTile = t;
    }

    public Tile GetCurrentTile() {
        return this.CurrentTile;
    }

    public Dictionary<string, Item> GetInventory() {
        return this.Inventory;
    }
}
