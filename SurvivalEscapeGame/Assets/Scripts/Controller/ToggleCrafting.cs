﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleCrafting : MonoBehaviour, IPointerDownHandler {
    [SerializeField]
    public GameObject CraftingPanel;

    public void OnPointerDown(PointerEventData eventData) {
        PointerDownAction();
    }

    public void PointerDownAction() {
        CraftingPanel.SetActive(!CraftingPanel.activeSelf);
        Vector3 rotation = this.gameObject.GetComponent<RectTransform>().eulerAngles;
        this.gameObject.GetComponent<RectTransform>().eulerAngles = new Vector3(0.0f, 0.0f, rotation.z + 180);
    }
}
