using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToggleEncyclo : MonoBehaviour, IPointerDownHandler {
    [SerializeField]
    private GameObject EncycloPanel;

    public void OnPointerDown(PointerEventData eventData) {
        EncycloPanel.SetActive(!EncycloPanel.activeSelf);
    }

    public bool IsActive() {
        return EncycloPanel.activeSelf;
    }

    public void SetActive(bool active) {
        EncycloPanel.SetActive(active);
    } 
}
