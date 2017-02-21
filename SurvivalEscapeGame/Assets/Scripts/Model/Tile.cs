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
	public TileType Type;

	public Tile(int id, TileType type) {
		this.Id = id;
		this.Type = type;
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

}
