﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item {
    public static int IdCounter = 0;

    protected string Name;
    public Sprite Icon { get; set; }
    public int MaximumQuantity { get; set; }
    public int Slot { get; set; }
    protected string Description;
    protected int Id;
    protected int DepthLevel; //Tiledepth of 5, DepthLevel 2 -> 3 digs
    protected bool Active;
    protected int Quantity;
    public GameObject ItemObject { get; set; }

    public Item(int id, int depthLevel, bool active) : this(id, depthLevel, active, 1) {
    }

    public Item(int id, int depthLevel, bool active, int quantity) {
        this.Id = id;
        this.DepthLevel = depthLevel;
        this.Active = active;
        this.SetQuantity(quantity);
    }

    public Item(int id, bool active) : this(id, 0, active, 1) {
    }

    public Item(int id, bool active, int quantity) : this(id, 0, active, quantity) {
    }

    public bool IsActive() {
        return this.Active;
    }

    public void SetActive(bool active) {
        this.Active = active;
    }

    public int GetId() {
        return this.Id;
    }

    public int GetDepthLevel() {
        return this.DepthLevel;
    }

    public string GetName() {
        return this.Name;
    }

    public int GetQuantity() {
        return this.Quantity;
    }

    public void SetQuantity(int quantity) {
        this.Quantity = quantity;
    }
}
