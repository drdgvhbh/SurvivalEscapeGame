using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryInput : MonoBehaviour, IPointerDownHandler {
    [SerializeField]
    private GameObject InventoryPanel;

    public void OnPointerDown(PointerEventData eventData) {
        InventoryPanel.SetActive(!InventoryPanel.activeSelf);
    }

    public bool IsActive() {
        return InventoryPanel.activeSelf;
    }

    public void SetActive(bool active) {
        InventoryPanel.SetActive(active);
    }
}
