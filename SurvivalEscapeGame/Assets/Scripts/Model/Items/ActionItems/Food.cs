using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Food : ActionItem {
    public float NourishmentReplenishment;
    public float HealthReplenishment;
    public float StaminaReplenishment;
    public Food(int id, int depthLevel, bool active) : base(id, depthLevel, active) {

    }

    public Food(int id, int depthLevel, bool active, int quantity) : base(id, depthLevel, active, quantity) {

    }

    public Food(int id, bool active) : base(id, active) {

    }

    public Food(Food food) : base(food) {
    }

    public void Eat(PlayerData pd) {
        Debug.Log(NourishmentReplenishment + " " + StaminaCost);
        pd.Stamina = pd.Stamina - StaminaCost;
        pd.NourishmentStatus = pd.NourishmentStatus + NourishmentReplenishment;
        pd.Health += HealthReplenishment;
        pd.Stamina += StaminaReplenishment;
        pd.RemoveItem(this, 1, pd.GetInventory());

        pd.GUIText.GetComponent<Text>().text = "Consumed for " + NourishmentReplenishment + "N, " 
            + HealthReplenishment + "H, " 
            + StaminaReplenishment + "S.";
    }
}
