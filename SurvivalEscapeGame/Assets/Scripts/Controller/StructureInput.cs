using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StructureInput : MonoBehaviour, IPointerDownHandler {
    private PlayerInput Pi;
    private PlayerData Pd;
    KeyCode Hotkey = KeyCode.Space;

    [SerializeField]
    private GameObject StructurePanel;
    [SerializeField]
    private GameObject StructureText;

    [SerializeField]
    private GameObject ToggleCrafting;

    protected void Awake() {
        this.Pi = GameObject.Find("Player").GetComponent<PlayerInput>();
        this.Pd = GameObject.Find("Player").GetComponent<PlayerData>();
    }

    protected void Start() {

    }

    private void Update() {
        if (Pd.CurrentTile.Structure.Value == null) {
            this.GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            StructurePanel.SetActive(false);
            DestroyStructureSlotChildren();
        } else {
            this.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            if (Input.GetKeyDown(Hotkey)) {
                ExecuteEvents.Execute(this.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                ButtonAction();
            }
        }
        if (StructurePanel.activeSelf && ToggleCrafting.GetComponent<ToggleCrafting>().CraftingPanel.activeSelf) {
            ToggleCrafting.GetComponent<ToggleCrafting>().PointerDownAction();
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        ButtonAction();
    }

    public void ButtonAction() {
        GameObject structure = Pd.CurrentTile.Structure.Value;
        if (Pd.CurrentTile.Structure.Value == null)
            return;
        if (!StructurePanel.activeSelf) {
            Time.timeScale = 0.0f;
        } else {
            Time.timeScale = 1.0f;
        }
        Pd.CurrentTile.Structure.Value.GetComponent<StructureData>().ItemContainer = new List<GameObject>();
        List<GameObject> itemContainer = Pd.CurrentTile.Structure.Value.GetComponent<StructureData>().ItemContainer;
        for (int i = 0; i < itemContainer.Count; i++) {
            itemContainer[i] = null;
        }
        StructurePanel.SetActive(!StructurePanel.activeSelf);
        StructureText.GetComponent<TextMeshProUGUI>().text = Pd.CurrentTile.Structure.Value.GetComponent<StructureData>().Name;
        Dictionary<String, Item> itemInventory = Pd.CurrentTile.Structure.Value.GetComponent<StructureData>().Inventory;
        DestroyStructureSlotChildren();
        int counter = 0;
        foreach (KeyValuePair<String, Item> i in itemInventory) {
            PlayerData.StructureSlots[counter].GetComponent<SlotInput>().PopulateSlot(i.Value, itemContainer, itemInventory, counter);
            counter++;
        }
        Pd.CurrentTile.Structure.Value.GetComponent<StructureData>().UpdateOnOpen();
    }

    public void DestroyStructureSlotChildren() {
        foreach (GameObject g in PlayerData.StructureSlots) {
            for (int i = 0; i < g.transform.childCount; i++) {
                GameObject.Destroy(g.transform.GetChild(i).gameObject);
            }
        }
    }

}
