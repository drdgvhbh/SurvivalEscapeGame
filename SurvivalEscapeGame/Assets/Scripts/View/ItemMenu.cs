using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour {
    [SerializeField]
    private GameObject DropDownMenu;
    private Dropdown DropDown;

    private void Start() {
        DropDown = DropDownMenu.GetComponent<Dropdown>();
        DropDown.ClearOptions();
        List<string> itemNames = new List<string>(Global.ItemNames.Values);
        itemNames.Sort();
        DropDown.AddOptions(itemNames);
    }

    public void UpdateDropDownList() {

    } 
}
