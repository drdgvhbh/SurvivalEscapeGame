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
	public int Id;
	public Tile[] Neighbours = new Tile[4];
	public int AutoTileID;
    private Vector3 Position;
	public TileType Type;

	public Tile(int id, TileType type, Vector3 position) {
		this.Id = id;
		this.Type = type;
        this.SetPosition(position);
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
}
