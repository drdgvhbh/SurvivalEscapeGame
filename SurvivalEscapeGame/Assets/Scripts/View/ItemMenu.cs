using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemMenu : MonoBehaviour {
    [SerializeField]
    private GameObject DropDownMenu;
    [SerializeField]
    private GameObject Title;

    [SerializeField]
    private GameObject Image;
    [SerializeField]
    private GameObject Description;
    [SerializeField]
    private GameObject BasicEffects;
    [SerializeField]
    private GameObject AdvancedEffects;

    private void Start() {
        Dropdown DropDown = DropDownMenu.GetComponent<Dropdown>();
        DropDownMenu.GetComponent<Dropdown>().onValueChanged.AddListener(delegate {
            UpdateDropDownList(DropDown);
        });
        UpdateDropDownList(DropDown);
        
    }

    public void UpdateDropDownList(Dropdown dd) {
        Dropdown DropDown = dd;
        DropDown.ClearOptions();
        List<string> itemNames = new List<string>(Global.ItemNames.Values);
        itemNames.Sort();
        DropDown.AddOptions(itemNames);
        var thisTextNode = ItemDatabase.JsonNode["Items"][DropDown.options[DropDown.value].text];
        Title.GetComponent<Text>().text = thisTextNode["Name"];
        Description.GetComponent<Text>().text = thisTextNode["Description"];
        string BasicString =
            "<b>Maximum Quantity:</b> " + thisTextNode["MaximumQuantity"] + "\n"
            + "<b>Consumable:</b> " + thisTextNode["Consumable"] + "\n"
            + "<b>Locations:</b> ";
        for (int i = 0; i < thisTextNode["Locations"].AsArray.Count; i++) {
            if (i != thisTextNode["Locations"].AsArray.Count - 1) {
                BasicString = BasicString + thisTextNode["Locations"][i] + ", ";
            } else {
                BasicString = BasicString + thisTextNode["Locations"][i] + "\n";
            }
        }
        BasicString = BasicString            
            + "<b>Usable In:</b> " + thisTextNode["UsableIn"] + "\n";
        BasicEffects.GetComponent<Text>().text = BasicString;
        string AdvancedString =
            "<b>Stamina Cost:</b> " + thisTextNode["StaminaCost"] + "\n" 
            + "<b>Channel Duration:</b> " + thisTextNode["ChannelDuration"] + "s\n"
            +"<b>Crafting Components:</b> " + thisTextNode["Components"] + "\n";
        AdvancedEffects.GetComponent<Text>().text = AdvancedString;
        Image.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
    }
}
