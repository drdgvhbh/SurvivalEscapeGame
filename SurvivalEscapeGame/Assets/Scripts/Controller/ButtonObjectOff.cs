using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonObjectOff : MonoBehaviour, IPointerDownHandler {
    [SerializeField]
    private GameObject button;

    public void OnPointerDown(PointerEventData eventData) {
        button.SetActive(false);
        if (button.gameObject.name == "Help") {
            Time.timeScale = 1.0f;
        }
    }

    public bool IsActive() {
        return button.activeSelf;
    }

    public void SetActive(bool active) {
        button.SetActive(active);
    }
}
