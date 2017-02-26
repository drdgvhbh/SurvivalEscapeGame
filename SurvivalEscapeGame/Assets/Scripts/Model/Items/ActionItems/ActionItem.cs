using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionItem : Item {
    // Cost per second
    protected float HungerCost;
    protected float ThirstCost;

    //The duration in seconds
    protected float ChannelTime;

    public ActionItem(int id, int depthLevel, bool active) : base(id, depthLevel, active) {

    }

    public ActionItem(int id, bool active) : base(id, active) {

    }

    public float GetTotalThirstCost() {
        return this.ThirstCost * this.ChannelTime;
    }

    public float GetTotalHungerCost() {
        return this.HungerCost * this.ChannelTime;
    }
}
