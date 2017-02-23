using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {
	public static Dictionary<int, int> Sides = new Dictionary<int, int>() {
		{ 0, 1},			//Top
		{ 1, 1 << 1},       //Left
		{ 2, 1 << 2},       //Right
		{ 3, 1 << 3}		//Bottom
	};
    public static Dictionary<TileType, int> TileDepthTable = new Dictionary<TileType, int>() {
        { TileType.Empty, -1 },
        { TileType.Placeholder, 3},
        { TileType.Grass, 2}
    };
	public int Id;
	public Tile[] Neighbours = new Tile[4];
	public int AutoTileID;
    private Vector3 Position;
	public TileType Type;
    private int TileDepth;
    private List<Item> Items;

	public Tile(int id, TileType type, Vector3 position) {
		this.Id = id;
		this.Type = type;
        this.TileDepth = Tile.TileDepthTable[this.Type] + UnityEngine.Random.Range(Global.DepthVarianceMin, Global.DepthVarianceMax);
        this.SetPosition(position);
        this.Items = new List<Item>();
	}

    public Tile[] GetNeighbours() {
        return this.Neighbours;
    }

	public bool AddNeighbour(Sides side, Tile tile) {
		if (this.Neighbours[(int)side] == null) {
			this.Neighbours[(int)side] = tile;
			this.CalculateAutoTileID();
			return true;
		} else {
			return false;
		}
	}

	public void CalculateAutoTileID() {
		int sum = 0;
		for (int i = 0; i < Neighbours.Length; i++) { }
		foreach (Tile tile in this.Neighbours) {
            if (tile != null) {
                sum += Tile.Sides[Array.IndexOf(Neighbours, tile)];
            }
		}
		this.AutoTileID = sum;
    }

    public Vector3 GetPosition() {
        return new Vector3(this.Position.x, this.Position.y, this.Position.z);
    }

    public void SetPosition(Vector3 position) {
        this.Position = new Vector3(position.x, position.y, position.z);
    }

    public List<Item> GetItems() {
        return new List<Item>(this.Items);
    }

    public void AddItem(Item item) {
        this.Items.Add(item);
    }

    public Item RemoveItem(int id) {
        for (int i = 0; i < this.Items.Count; i++) {
            if (this.Items[i].GetId() == id) {
                Item it = this.Items[i];
                this.Items.RemoveAt(i);
                return it;
            }
        }
        return null;
    }

    public int GetTileDepth() {
        return this.TileDepth;
    }

    public void SetTileDepth(int depth) {
        this.TileDepth = depth;
    }
}
