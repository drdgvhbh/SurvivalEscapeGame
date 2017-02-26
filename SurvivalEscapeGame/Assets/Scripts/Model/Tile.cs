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
    public Vector3[] Norms;
    private bool Active;
    public int[] NormIdx;

	public Tile(int id, TileType type, Vector3 position, ref Vector3[] norms, int[] normIdx) {
		this.Id = id;
		this.Type = type;
        this.TileDepth = Tile.TileDepthTable[this.Type] + UnityEngine.Random.Range(Global.DepthVarianceMin, Global.DepthVarianceMax);
        this.SetPosition(position);
        this.Items = new List<Item>();
        this.Norms = norms;
        this.NormIdx = normIdx;
        this.Active = false;
	}

    public Tile[] GetNeighbours() {
        return this.Neighbours;
    }

    public List<Tile> GetExtendedNeighbours(int numExtended) {
        List<Tile> tiles = new List<Tile>();
        tiles.Add(this);
        for (int i = 0; i < numExtended; i++) {
            int c = tiles.Count;
            for (int k = 0; k < c; k++) {
                for (int j = 0; j < tiles[k].GetNeighbours().Length; j++) {
                    if (tiles[k].GetNeighbours()[j] == null || tiles.Contains(tiles[k].GetNeighbours()[j])) {
                        continue;
                    }
                    tiles.Add(tiles[k].GetNeighbours()[j]);
                }
            }
        }
       return tiles;
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

    public bool IsActive() {
        return this.Active;
    }

    public void SetActive(bool active) {
        this.Active = active;
    }
}
