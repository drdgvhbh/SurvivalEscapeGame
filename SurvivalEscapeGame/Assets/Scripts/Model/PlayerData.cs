﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SimpleJSON;

public class PlayerData : MonoBehaviour {
    private static JSONNode DataNode;
    public static JSONNode PlayerNode {
        get {
            if (DataNode == null) {
                string jsonString = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/PlayerData.json");
                DataNode = JSON.Parse(jsonString);
            }
            return DataNode;
        }
    }
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
    public float StaminaRegeneration;
    public bool Alive;
    public bool IsWeaponEquipped;
    public bool IsArmourEquipped;
    public int Direction = 1;
    public float Damage;
    public float AttackStaminaCost;
    public bool IsAttackOnCooldown;
    public float AttackCooldown = 0.33f;
    public float HealthBonus;
    public int VisionRange { get; private set; }

    private float MaximumNourishmentStatus;

    public Dictionary<PlayerActions, bool> PerformingAction;

    [SerializeField]
    private GameObject Model;

    [SerializeField]
    private GameObject HealthBar;
    [SerializeField]
    private GameObject NourishmentBar;
    [SerializeField]
    private GameObject StaminaBar;
    [SerializeField]
    private GameObject NourishmentText;

    public const int NumItemSlots = 16;
    public const int NumCraftingSlots = 6;
    public static int NumStructSlots = 8;
    [SerializeField]
    private GameObject SlotPanel;
    [SerializeField]
    private GameObject InventorySlot;
    [SerializeField]
    private GameObject InventoryItem;
    [SerializeField]
    private GameObject ActivePanel;
    [SerializeField]
    private GameObject CraftingPanel;

    [SerializeField]
    private GameObject StructurePanel;

    [SerializeField]
    public GameObject GUIText;

    [SerializeField]
    private GameObject HealthText;

    [SerializeField]
    private GameObject StaminaText;

    [SerializeField]
    private GameObject NourishmentTextStatus;

    public List<GameObject> FOWStructures;
    public HashSet<GameObject> AllStructures;

    [SerializeField]
    protected Sprite Death;

    [SerializeField]
    public AnimatorOverrideController SpearCtrl;
    [SerializeField]
    public AnimatorOverrideController ArmorCtrl;
    [SerializeField]
    public AnimatorOverrideController EquippedCtrl;

    private Dictionary<string, Item> Inventory;
    public static Dictionary<string, Item> CraftingInventory = new Dictionary<string, Item>();
    public static List<GameObject> Slots = new List<GameObject>();
    public static List<GameObject> CraftingSlots = new List<GameObject>();
    public static List<GameObject> StructureSlots = new List<GameObject>();
    public static List<GameObject> Items = new List<GameObject>();
    public static List<GameObject> CraftingItems = new List<GameObject>();
    private HashSet<Tile> DiscoveredTiles;

    [SerializeField]
    private GameObject GameGrid;

    // Use this for initialization
    private void Awake() {
        DiscoveredTiles = new HashSet<Tile>();
        VisionRange = PlayerNode["VisionRange"];
        for (int i = 0; i < PlayerData.NumItemSlots + PlayerData.NumCraftingSlots; i++) {
            GameObject Is = Instantiate(InventorySlot);
            PlayerData.Slots.Add(Is);
            PlayerData.Slots[i].GetComponent<SlotInput>().SlotID = i;
            if (i >= PlayerData.NumItemSlots) {
                PlayerData.CraftingSlots.Add(Is);
                PlayerData.CraftingSlots[i - PlayerData.NumItemSlots] = PlayerData.Slots[i];
                PlayerData.Slots[i].transform.SetParent(CraftingPanel.transform, false);
                PlayerData.CraftingSlots[i - PlayerData.NumItemSlots].GetComponent<SlotInput>().CraftingSlot = true;
            } else {
                PlayerData.Slots[i].transform.SetParent(this.SlotPanel.transform, false);
                PlayerData.Slots[i].GetComponent<SlotInput>().CraftingSlot = false;
            }
        }
        for (int i = 0; i < PlayerData.NumStructSlots; i++) {
            GameObject Is = Instantiate(InventorySlot);
            PlayerData.Slots.Add(Is);
            int idx = i + PlayerData.NumItemSlots + PlayerData.NumCraftingSlots;
            SlotInput sI = PlayerData.Slots[idx].GetComponent<SlotInput>();
            sI.SlotID = idx;
            sI.CraftingSlot = false;
            sI.IsStructureSlot = true;
            StructureSlots.Add(Is);
            StructureSlots[i].transform.SetParent(StructurePanel.transform, false);
            FOWStructures = new List<GameObject>();
            AllStructures = new HashSet<GameObject>();
        }
    }

    public void LateStart() {
        this.NourishmentLevel = 1;
        this.MaximumHealth = NourishmentLevels.BaseMaximumHealth[this.NourishmentLevel];
        this.Health = NourishmentLevels.BaseMaximumHealth[this.NourishmentLevel];
        this.MaximumStamina = NourishmentLevels.BaseMaximumStamina[this.NourishmentLevel];
        this.Stamina = NourishmentLevels.BaseMaximumStamina[this.NourishmentLevel];
        this.StaminaRegeneration = NourishmentLevels.BaseStaminaRegeneration[this.NourishmentLevel];
        this.MovementSpeed = NourishmentLevels.BaseMovementSpeed[this.NourishmentLevel];
        this.HealthRegeneration = NourishmentLevels.BaseHealthRegeneration[this.NourishmentLevel];
        this.NourishmentStatus = NourishmentLevels.NourishmentThreshold[this.NourishmentLevel] / 2;
        this.NourishmentDecayRate = NourishmentLevels.NourishmentDecayRate[this.NourishmentLevel];
        this.position = this.GetComponent<Transform>().position;
        HealthBonus = 0;
        this.Alive = true;
        this.IsPerformingAction = false;
        IsAttackOnCooldown = false;
        Damage = 20.0f;
        AttackStaminaCost = 10.0f;
        this.PerformingAction = new Dictionary<PlayerActions, bool>() {
            {PlayerActions.Move, false },
            {PlayerActions.Dig, false },
            {PlayerActions.BuildTent, false },
            {PlayerActions.Attack, false },
            {PlayerActions.Eat, false },
            {PlayerActions.BuildGranary, false },
            {PlayerActions.BuildWall, false },
            {PlayerActions.UseSpear, false },
            {PlayerActions.UseRadar, false },
            {PlayerActions.UseBeacon, false},
            {PlayerActions.UseHeavyArmour, false}
        };
        this.Inventory = new Dictionary<string, Item>();
        this.GetComponent<PlayerFogOfWar>().UpdateFogOfWar();
        this.HealthBar.GetComponent<Image>().fillAmount = this.Health / this.MaximumHealth;
        this.MaximumNourishmentStatus = 0f;
        for (int i = -2; i <= 2; i++) {
            this.MaximumNourishmentStatus += NourishmentLevels.NourishmentThreshold[i];
        }
        SlotInput.Pd = this;

        this.AddItem(new Shovel(++Item.IdCounter, true), this.GetInventory(), NumItemSlots, Slots, Items);
        this.AddItem(new Tent(++Item.IdCounter, true), this.GetInventory(), NumItemSlots, Slots, Items);
        this.AddItem(new Coconut(++Item.IdCounter, true), this.GetInventory(), NumItemSlots, Slots, Items);
        this.AddItem(new Coconut(++Item.IdCounter, true), this.GetInventory(), NumItemSlots, Slots, Items);
        this.AddItem(new Wood(++Item.IdCounter, true), this.GetInventory(), NumItemSlots, Slots, Items);
        this.AddItem(new Wood(++Item.IdCounter, true), this.GetInventory(), NumItemSlots, Slots, Items);
        this.AddItem(new Wall(++Item.IdCounter, true), this.GetInventory(), NumItemSlots, Slots, Items);
        this.AddItem(new Wall(++Item.IdCounter, true), this.GetInventory(), NumItemSlots, Slots, Items);
        this.AddItem(new Wall(++Item.IdCounter, true), this.GetInventory(), NumItemSlots, Slots, Items);
        this.AddItem(new Wall(++Item.IdCounter, true), this.GetInventory(), NumItemSlots, Slots, Items);
        //
    }

    // Update is called once per frame
    private void Update() {
        this.MaximumStamina = NourishmentLevels.BaseMaximumStamina[this.NourishmentLevel];
        this.MovementSpeed = NourishmentLevels.BaseMovementSpeed[this.NourishmentLevel];
        this.HealthRegeneration = NourishmentLevels.BaseHealthRegeneration[this.NourishmentLevel];
        this.MaximumHealth = NourishmentLevels.BaseMaximumHealth[this.NourishmentLevel];
        this.UpdateHealth();
        this.ApplyNourishmentDecay();
        this.UpdateNourishmentStatus();
        this.UpdateStamina();
    }   

    public void UpdateStamina() {
        this.StaminaRegeneration = NourishmentLevels.BaseStaminaRegeneration[this.NourishmentLevel];
        if (CurrentTile.Structure.Key == ItemList.Tent) {
            this.StaminaRegeneration += MaximumStamina * 0.2f;
        }
        this.StaminaRegeneration *= Time.deltaTime;

        this.Stamina = System.Math.Min(MaximumStamina, this.Stamina + this.StaminaRegeneration);

        this.StaminaBar.GetComponent<Image>().fillAmount = this.Stamina / this.MaximumStamina;
        StaminaText.GetComponent<TextMeshProUGUI>().text = System.Math.Ceiling(Stamina) + " / " + MaximumStamina.ToString("F0");

    }

    public void UpdateHealth() {
        float maxHealth = MaximumHealth + HealthBonus;
        this.Health = System.Math.Min(maxHealth, this.Health + (this.HealthRegeneration * Time.deltaTime));
        this.HealthBar.GetComponent<Image>().fillAmount = this.Health / maxHealth;
        HealthText.GetComponent<TextMeshProUGUI>().text = System.Math.Ceiling(Health) + " / " + maxHealth.ToString("F0");
    }

    public void ApplyNourishmentDecay() {
        if (this.NourishmentStatus > 0) {
            this.NourishmentStatus = this.NourishmentStatus - (this.NourishmentDecayRate * Time.deltaTime);
        } else if (this.NourishmentLevel == -2) {
            this.NourishmentStatus = 0;
        }
        NourishmentText.GetComponent<TextMeshProUGUI>().text = (this.NourishmentLevel + 2).ToString();
    }

    public void UpdateNourishmentStatus() {
        if (this.NourishmentLevel == 2 && this.NourishmentStatus > NourishmentLevels.NourishmentThreshold[2]) {
            this.NourishmentStatus = NourishmentLevels.NourishmentThreshold[2];
        }
        if (this.NourishmentLevel > -2 && this.NourishmentStatus <= 0) {
            this.NourishmentLevel--;
            this.NourishmentStatus = NourishmentLevels.NourishmentThreshold[this.NourishmentLevel] + this.NourishmentStatus;
        } else if (this.NourishmentLevel < 2 && this.NourishmentStatus > NourishmentLevels.NourishmentThreshold[this.NourishmentLevel]) {
            this.NourishmentLevel++;
            this.NourishmentStatus = NourishmentLevels.NourishmentThreshold[this.NourishmentLevel] - this.NourishmentStatus;
        }
        this.NourishmentBar.GetComponent<Image>().fillAmount = this.NourishmentStatus / NourishmentLevels.NourishmentThreshold[this.NourishmentLevel];
        NourishmentTextStatus.GetComponent<TextMeshProUGUI>().text = System.Math.Ceiling(NourishmentStatus) + " / " + NourishmentLevels.NourishmentThreshold[this.NourishmentLevel].ToString("F2");
    }

    public void DamagePlayer(float damage) {
        float oldHealth = Health;
        this.Health = this.Health - damage;
        GUIText.GetComponent<Text>().text = "You took " + damage + " damage! Health: " + oldHealth.ToString("F2") + "->" + Health.ToString("F2");
        Animator animCtrl = this.gameObject.GetComponent<Animator>();
        try {
            foreach (KeyValuePair<PlayerInput.PlayerAnimationActions, string> e in PlayerInput.AnimationActions) {
                animCtrl.ResetTrigger(e.Value);
            }
            if (this.GetComponent<PlayerInput>().Direction != Tile.Sides.Left && this.GetComponent<PlayerInput>().Direction != Tile.Sides.Top) {
                animCtrl.SetTrigger(PlayerInput.AnimationActions[PlayerInput.PlayerAnimationActions.IdleRight]);
                animCtrl.SetTrigger(PlayerInput.AnimationActions[PlayerInput.PlayerAnimationActions.DamagedRight]);
            } else if (this.GetComponent<PlayerInput>().Direction == Tile.Sides.Top) {
                animCtrl.SetTrigger(PlayerInput.AnimationActions[PlayerInput.PlayerAnimationActions.MoveRight]);
                animCtrl.SetTrigger(PlayerInput.AnimationActions[PlayerInput.PlayerAnimationActions.IdleRight]);
                animCtrl.SetTrigger(PlayerInput.AnimationActions[PlayerInput.PlayerAnimationActions.DamagedRight]);
                this.GetComponent<PlayerInput>().Direction = Tile.Sides.Right;
            } else {
                animCtrl.SetTrigger(PlayerInput.AnimationActions[PlayerInput.PlayerAnimationActions.IdleLeft]);
                animCtrl.SetTrigger(PlayerInput.AnimationActions[PlayerInput.PlayerAnimationActions.DamagedLeft]);
            }
        }
        catch (MissingComponentException e) {
            Debug.Log("Player should be dead because there is no animator");
        }
        if (this.Health <= 0 && Alive == true) {
            Transform camera = this.gameObject.transform.GetChild(0);
            camera.SetParent(this.gameObject.transform.parent.gameObject.transform);
            UpdateHealth();
            this.Alive = false;
            Destroy(animCtrl);
            GetComponent<SpriteRenderer>().sprite = Death;
            GetComponent<PlayerInput>().enabled = false;
            StartCoroutine(Killme(5.0f));
            GUIText.GetComponent<Text>().text = "You are dead!!!";
    
        }
    }

    private IEnumerator Killme(float waitTime) {
        yield return new WaitForSecondsRealtime(waitTime);
        Destroy(this.gameObject);
    }


    public bool AddItem(Item it) {
        return AddItem(it, this.GetInventory(), NumItemSlots, Slots, Items);
    }

    public bool AddItem(Item it, Dictionary<string, Item> inventory, int maximumSlotNumber, List<GameObject> SlotContainer,
        List<GameObject> ItemContainer) {
        return AddItem(it, inventory, maximumSlotNumber, SlotContainer, ItemContainer, false);
    }

    public bool AddItem(Item it, Dictionary<string, Item> inventory, int maximumSlotNumber, List<GameObject> SlotContainer, 
        List<GameObject> ItemContainer, bool ignoreContains) {
        if (inventory.Count > maximumSlotNumber) {
            return false;
        }
        if (!inventory.ContainsKey(it.GetName())) {
            inventory.Add(it.GetName(), it);
            for (int i = 0; i < maximumSlotNumber; i++) {
                if (SlotContainer[i].transform.childCount == 0) {
                    GameObject item = SlotContainer[i].GetComponent<SlotInput>().PopulateSlot(it, ItemContainer, inventory, i);
                    if (inventory == this.GetInventory()) {
                        FillActiveSlot(it, item);
                    }
                    return true;
                }
            }
        } else if ((PlayerData.CraftingInventory.ContainsKey(it.GetName()) && this.GetInventory()[it.GetName()].GetQuantity() 
                + PlayerData.CraftingInventory[it.GetName()].GetQuantity() < inventory[it.GetName()].MaximumQuantity) 
            || (PlayerData.CraftingInventory.ContainsKey(it.GetName()) == false && this.GetInventory()[it.GetName()].GetQuantity()
                < inventory[it.GetName()].MaximumQuantity) || ignoreContains) {
            Item tmp = inventory[it.GetName()];
            tmp.SetQuantity(tmp.GetQuantity() + 1);
            tmp.ItemObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = tmp.GetQuantity().ToString();
            return true;
        } else {
            GUIText.GetComponent<Text>().text = "Maximum quantity reached!";
            Debug.Log("You've reached the maximum quantity of this item! (" + it.GetName()+")");
        }
        return false;
    }

    public void FillActiveSlot(Item it, GameObject item) {
        int numActive = ActivePanel.transform.childCount;
        for (int j = 0; j < numActive; j++) {
            Transform activeSlot = ActivePanel.transform.GetChild(j);
            if (it.GetType().IsSubclassOf(typeof(ActionItem)) && activeSlot.GetComponent<ActiveInput>().Item == null) {
                activeSlot.GetComponent<ActiveInput>().Item = it;
                activeSlot.GetComponent<ActiveInput>().Slot = j;
                activeSlot.GetChild(0).GetComponent<Image>().sprite = Item.BorderSprite;
                activeSlot.GetChild(0).GetComponent<Image>().color = new Color(255, 255, 255, 255);
                activeSlot.GetChild(1).GetComponent<Image>().sprite = item.GetComponent<Image>().sprite;
                activeSlot.GetChild(1).GetComponent<Image>().color = new Color(255, 255, 255, 255);
                UpdateActiveHotkey(activeSlot);
                activeSlot.GetChild(2).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
                activeSlot.GetChild(2).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
               
                ActionItem ai = (ActionItem)it;
                activeSlot.GetChild(3).GetComponent<TextMeshProUGUI>().text = ai.StaminaCost.ToString();
                activeSlot.GetChild(3).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
                activeSlot.GetChild(3).GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Right;
                it.ActiveContainer = activeSlot.gameObject;
                break;
            }
        }
    }

    public void UpdateActiveHotkey(Transform t) {
        t.GetChild(2).GetComponent<TextMeshProUGUI>().text = ActiveInput.Hotkeys[t.GetComponent<ActiveInput>().Slot].ToString();
    }

    public void RemoveItem(Item it, int quantity, Dictionary<string, Item> inventory) {
        if (inventory.ContainsKey(it.GetName())) {
            Item itInvent = inventory[it.GetName()];
            itInvent.SetQuantity(itInvent.GetQuantity() - quantity);
            itInvent.ItemObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = itInvent.GetQuantity().ToString();
            if (itInvent.GetQuantity() <= 0) {
                inventory.Remove(it.GetName());
                Destroy(it.ItemObject);
                if (it.GetType().IsSubclassOf(typeof(ActionItem)) && it.ActiveContainer != null) {
                    Model.GetComponent<Model>().activePanels.Remove(it.ActiveContainer);
                    Destroy(it.ActiveContainer);
                    Model.GetComponent<Model>().CreateActivePanel();
                    for (int i = 0; i < Model.GetComponent<Model>().activePanels.Count; i++) {
                        GameObject activePanel = Model.GetComponent<Model>().activePanels[i];
                        activePanel.GetComponent<ActiveInput>().SetSlot(i);
                        if (activePanel.GetComponent<ActiveInput>().Item != null) {
                            UpdateActiveHotkey(activePanel.transform);
                        }
                    }
                }
                
            }
        } else {
            Debug.Log("Item does not exist in player's invetory!");
        }
    }

    public void RemoveItem(Item it, Dictionary<string, Item> inventory) {
        RemoveItem(it, it.GetQuantity(), inventory);
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

    public void CalculateCurrentTIle() {
        int oldIdx = CurrentTile.Index;
        int idx = TerrainData.GetIndexFromPosition(this.transform.position, GameGrid.GetComponent<Grid>());
        Tile[] Tiles = GameGrid.GetComponent<TerrainData>().Tiles;
        if (Tiles[idx].IsWalkable) {
            CurrentTile = Tiles[idx];
            if (idx != oldIdx) {
                this.GetComponent<PlayerInput>().Move();
                Tiles[oldIdx].CurrentGameObject = null;
                Tiles[idx].CurrentGameObject = this.gameObject;
            }
        }
    }

    public void DiscoverTiles() {
        GetComponent<PlayerFogOfWar>().UpdateFogOfWar();
        foreach (Tile t in GetRevealedTiles()) {
            if (t.CurrentGameObject != null ) {
                t.CurrentGameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
            }
            if (t.Savior != null) {
                t.Savior.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
            }
        }
    }

    public HashSet<Tile> GetRevealedTiles() {
        IEnumerator<GameObject> fowStructures = FOWStructures.GetEnumerator();
        HashSet<Tile> AllRevealedTiles = new HashSet<Tile>();
        bool cont = true;
        Tile ct = this.CurrentTile;
        int size = VisionRange;
        do {
            if (size < 0)
                return new HashSet<Tile>();
            HashSet<Tile> RevealedTiles = new HashSet<Tile>();
            ct.IsDiscovered = true;
            DiscoveredTiles.Add(ct);
            RevealedTiles.Add(ct);
            ct.IsRevealed = true;
            HashSet<Tile> temp = new HashSet<Tile>();
            for (int i = 0; i < size; i++) {
                temp = new HashSet<Tile>(RevealedTiles);
                foreach (Tile t in temp) {
                    Tile left = AddDiscoveredTile(t, Tile.Sides.Left, RevealedTiles);
                    Tile right = AddDiscoveredTile(t, Tile.Sides.Right, RevealedTiles);
                    Tile top = AddDiscoveredTile(t, Tile.Sides.Top, RevealedTiles);
                    Tile bottom = AddDiscoveredTile(t, Tile.Sides.Bottom, RevealedTiles);

                    if (left != null) {
                        AddDiscoveredTile(left, Tile.Sides.Bottom, RevealedTiles);
                    } else if (bottom != null) {
                        AddDiscoveredTile(bottom, Tile.Sides.Left, RevealedTiles);
                    }

                    if (bottom != null) {
                        AddDiscoveredTile(bottom, Tile.Sides.Right, RevealedTiles);
                    } else if (right != null) {
                        AddDiscoveredTile(right, Tile.Sides.Bottom, RevealedTiles);
                    }

                    if (right != null) {
                        AddDiscoveredTile(right, Tile.Sides.Top, RevealedTiles);
                    } else if (top != null) {
                        AddDiscoveredTile(top, Tile.Sides.Right, RevealedTiles);
                    }

                    if (top != null) {
                        AddDiscoveredTile(top, Tile.Sides.Left, RevealedTiles);
                    } else if (left != null) {
                        AddDiscoveredTile(left, Tile.Sides.Top, RevealedTiles);
                    }
                }
            }

            if (fowStructures.MoveNext()) {
                if (ct == null) {
                    fowStructures.Reset();
                }
                ct = fowStructures.Current.GetComponent<StructureData>().CurrentTile;
                size = 1;
            } else {
                cont = false;
            }
            AllRevealedTiles.UnionWith(RevealedTiles);
        } while (cont == true);
        return AllRevealedTiles;
    }

    private Tile AddDiscoveredTile(Tile t, Tile.Sides side, HashSet<Tile> revealed) {
        if (t != null && t.Neighbours.ContainsKey(side)) {
            DiscoveredTiles.Add(t.Neighbours[side]);
            revealed.Add(t.Neighbours[side]);
            t.Neighbours[side].IsDiscovered = true;
            t.Neighbours[side].IsRevealed = true;
            return t.Neighbours[side];
        }
        return null;
    }
}
