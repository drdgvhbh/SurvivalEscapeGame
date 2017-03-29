using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryInput : MonoBehaviour, IPointerDownHandler {
    [SerializeField]
    private GameObject InventoryPanel;

    public void OnPointerDown(PointerEventData eventData) {
        if (!InventoryPanel.activeSelf) {
            Time.timeScale = 0.0f;
        } else {
            Time.timeScale = 1.0f;
        }
        InventoryPanel.SetActive(!InventoryPanel.activeSelf);
    }

    public bool IsActive() {
        return InventoryPanel.activeSelf;
    }

    public void SetActive(bool active) {
        InventoryPanel.SetActive(active);
    }
}
