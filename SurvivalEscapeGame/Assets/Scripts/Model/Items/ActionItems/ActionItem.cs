using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionItem : Item { 
    public float StaminaCost;
    //The duration in seconds
    public float ChannelDuration;



    public ActionItem(int id, int depthLevel, bool active) : base(id, depthLevel, active) {

    }

    public ActionItem(int id, bool active) : base(id, active) {

    }

}
