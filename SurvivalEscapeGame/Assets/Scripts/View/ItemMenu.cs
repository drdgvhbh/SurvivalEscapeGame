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
        for (int i = 0; i < thisTextNode["Locations"].Count; i++) {
            if (i != thisTextNode["Locations"].Count - 1) {
                BasicString = BasicString + thisTextNode["Locations"][i] + ", ";
            } else {
                BasicString = BasicString + thisTextNode["Locations"][i];
            }
        }
        BasicString = BasicString
            + "\n"
            + "<b>Usable In:</b> ";
        for (int i = 0; i < thisTextNode["UsableIn"].Count; i++) {
            if (i != thisTextNode["UsableIn"].Count - 1) {
                BasicString = BasicString + thisTextNode["UsableIn"][i] + ", ";
            } else {
                BasicString = BasicString + thisTextNode["UsableIn"][i];
            }
        }

        BasicEffects.GetComponent<Text>().text = BasicString;
        string AdvancedString =
            "<b>Stamina Cost:</b> " + thisTextNode["StaminaCost"] + "\n"
            + "<b>Channel Duration:</b> " + thisTextNode["ChannelDuration"] + "s\n"
            + "<b>Crafting Components:</b> ";
        for (int i = 0; i < thisTextNode["Components"].Count; i++) {
            AdvancedString = AdvancedString +
                thisTextNode["Components"][i.ToString()]["Type"]
                + " <b>x</b> "
                + thisTextNode["Components"][i.ToString()]["Quantity"];
            if (i != thisTextNode["Components"].Count - 1) {
                AdvancedString += ", ";
            }
        }
        AdvancedString +=
            "\n"
            + "<b>Damage:</b> " + thisTextNode["Damage"] + "\n"
            + "<b>Nourishment Replenishment:</b> " + thisTextNode["NourishmentReplenishment"] + "\n"
            + "<b>Health Replenishment:</b> " + thisTextNode["HealthReplenishment"] + "\n"
            + "<b>Stamina Replenishment:</b> " + thisTextNode["StaminaReplenishment"] + "\n";
        AdvancedEffects.GetComponent<Text>().text = AdvancedString;
        Image.GetComponent<Image>().sprite = Resources.LoadAll<Sprite>(thisTextNode["Icon"])[thisTextNode["IconIndex"]];
    }
}
