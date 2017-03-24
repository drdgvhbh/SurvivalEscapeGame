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

    private void Awake() {
        this.Pi = GameObject.Find("Player").GetComponent<PlayerInput>();
        this.Pd = GameObject.Find("Player").GetComponent<PlayerData>();
    }

    private void Start() {

    }

    private void Update() {
        if (Pd.CurrentTile.Structure.Value == null) {
            this.GetComponent<Image>().color = new Color32(100, 100, 100, 255);
            StructurePanel.SetActive(false);
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
        StructurePanel.SetActive(!StructurePanel.activeSelf);
        StructureText.GetComponent<TextMeshProUGUI>().text = Pd.CurrentTile.Structure.Value.GetComponent<StructureData>().Name;
    }
}
