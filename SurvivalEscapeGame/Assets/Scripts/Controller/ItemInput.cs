using UnityEngine;
using UnityEngine.EventSystems;

public class ItemInput : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler ,IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {
    public static GameObject Tooltip;
    public Item Item;

    public Transform OriginalParent { get; private set; }
    private Vector2 Offset;

    private void Awake() {
        if (ItemInput.Tooltip == null)
            ItemInput.Tooltip = GameObject.FindGameObjectWithTag("Tooltip");
    }
    private void Start() {
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (this.Item == null)
            return;
        this.Offset = eventData.position - (Vector2)this.transform.position;
        this.OriginalParent = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);
        this.transform.position = eventData.position;
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;               
    }

    public void OnDrag(PointerEventData eventData) {
        if (this.Item == null)
            return;
        this.transform.position = eventData.position - this.Offset;        
    }

    public void OnEndDrag(PointerEventData eventData) {
        this.transform.SetParent(PlayerData.Slots[Item.Slot].transform);
        this.transform.position = PlayerData.Slots[Item.Slot].transform.position;
        this.GetComponent<CanvasGroup>().blocksRaycasts = true;
        
    }

    public void OnPointerEnter(PointerEventData eventData) {
        ItemInput.Tooltip.GetComponent<Tooltip>().Activate(this.Item);
    }

    public void OnPointerExit(PointerEventData eventData) {
        ItemInput.Tooltip.GetComponent<Tooltip>().DeActivate();
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (this.transform.parent.childCount > 0)
            this.OnEndDrag(eventData);
    }
}
