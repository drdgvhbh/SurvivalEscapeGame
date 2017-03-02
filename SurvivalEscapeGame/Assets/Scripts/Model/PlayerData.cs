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
    [SerializeField]
    private GameObject ActivePanel;

    public static List<GameObject> Slots = new List<GameObject>();
    public static List<GameObject> Items = new List<GameObject>();

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
        this.UpdateTileVisibility();
        this.HealthBar.GetComponent<Image>().fillAmount = this.Health / this.MaximumHealth;
        this.MaximumNourishmentStatus = 0f;
        for (int i = -2; i <= 2; i++) {
            this.MaximumNourishmentStatus += NourishmentLevels.NourishmentThreshold[i];
        }
        for (int i = 0; i < PlayerData.NumItemSlots; i++) {
            PlayerData.Slots.Add(Instantiate(InventorySlot));
            PlayerData.Slots[i].transform.SetParent(this.SlotPanel.transform);
            PlayerData.Slots[i].AddComponent<SlotInput>();
            PlayerData.Slots[i].GetComponent<SlotInput>().SlotID = i;
        }
        this.AddItem(new Shovel(++Item.IdCounter, true));
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

    public bool AddItem(Item it) {
        if (this.GetInventory().Count > PlayerData.NumItemSlots) {
            return false;
        }
        if (!this.GetInventory().ContainsKey(it.GetName())) {
            this.GetInventory().Add(it.GetName(), it);
            for (int i = 0; i < PlayerData.NumItemSlots; i++) {
                if (PlayerData.Slots[i].transform.childCount == 0) {
                    GameObject item = Instantiate(this.InventoryItem);
                    Items.Add(item);
                    this.GetInventory()[it.GetName()].ItemObject = item;
                    item.transform.SetParent(PlayerData.Slots[i].transform);
                    item.transform.localPosition = Vector3.zero;
                    item.GetComponent<Image>().sprite = this.GetInventory()[it.GetName()].Icon;
                    item.transform.GetChild(0).GetComponent<Text>().text = it.GetQuantity().ToString();
                    item.GetComponent<ItemInput>().Item = it;
                    it.Slot = i;
                    PlayerData.Slots[i].GetComponent<SlotInput>().StoredItem = it;
                    int numActive = ActivePanel.transform.childCount;
                    for (int j = 0; j < numActive; j++) {                        
                        if (it.GetType().IsSubclassOf(typeof(ActionItem))) {
                            Transform activeSlot = ActivePanel.transform.GetChild(j);
                            activeSlot.GetComponent<ActiveInput>().Item = it;
                            activeSlot.GetComponent<ActiveInput>().Slot = j;
                            activeSlot.GetChild(0).GetComponent<Image>().sprite = Item.BorderSprite;
                            activeSlot.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 255);
                            activeSlot.GetChild(1).GetComponent<Image>().sprite = item.GetComponent<Image>().sprite;
                            activeSlot.GetChild(1).GetComponent<Image>().color = new Color(255, 255, 255, 255);
                            activeSlot.GetChild(2).GetComponent<Text>().text = ActiveInput.Hotkeys[activeSlot.GetComponent<ActiveInput>().Slot].ToString();
                            break;
                        }                 
                    }
                    return true;
                }
            }
        } else if (this.GetInventory()[it.GetName()].GetQuantity() < this.GetInventory()[it.GetName()].MaximumQuantity) {
            Item tmp = this.GetInventory()[it.GetName()];
            tmp.SetQuantity(tmp.GetQuantity() + 1);
            tmp.ItemObject.transform.GetChild(0).GetComponent<Text>().text = tmp.GetQuantity().ToString();
            return true;
        } else {
            Debug.Log("You've reached the maximum quantity of this item!");            
        }
        return false;
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
