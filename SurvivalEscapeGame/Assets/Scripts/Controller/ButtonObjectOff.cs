using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonObjectOff : MonoBehaviour, IPointerDownHandler {
    [SerializeField]
    private GameObject button;

    public void OnPointerDown(PointerEventData eventData) {
        button.SetActive(false);
    }

    public bool IsActive() {
        return button.activeSelf;
    }

    public void SetActive(bool active) {
        button.SetActive(active);
    }
}
