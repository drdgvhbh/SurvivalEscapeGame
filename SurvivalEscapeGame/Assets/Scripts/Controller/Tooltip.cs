using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {
    private Item Item;
    private GameObject TooltipObj;

    private void Awake() {
        this.TooltipObj = this.gameObject;
        ItemInput.Tooltip = this.gameObject;             
    }

    private void Start() {
        ItemInput.Tooltip.SetActive(false);
    }

    private void Update() {
        if (this.TooltipObj.activeSelf) {
            this.TooltipObj.transform.position = Input.mousePosition;
        }
    }

    public void Activate(Item Item) {
        this.Item = Item;
        this.ConstructDataString();
        this.TooltipObj.SetActive(true);
    }

    public void DeActivate() {
        this.TooltipObj.SetActive(false);
    }

    public void ConstructDataString() {
        this.TooltipObj.GetComponentInChildren<Text>().text = this.Item.GetName();
    }
}
