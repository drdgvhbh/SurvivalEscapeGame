using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBuilder {
	private int NumTiles;
	private int Rows;
	private int Columns;
    private float TileSize;
	private Tile[] Tiles; 

	public TerrainBuilder(int numTiles, int rows, int columns, float tileSize) {
		this.NumTiles = numTiles;
		this.Rows = rows;
		this.Columns = columns;
        this.TileSize = tileSize;
		this.Tiles = new Tile[this.NumTiles];
	}

	public Tile[] CreateTerrain() {
		for (int i = 0; i < NumTiles; i++) {
            int currentRow = i / this.Columns;
            //Debug.Log(currentRow);
            int currentColumn = i % this.Columns;
            Vector3 position = Global.Origin + Global.Offset + new Vector3(this.TileSize * currentColumn, -this.TileSize * currentRow);
            //Debug.Log(position);
			this.Tiles[i] = new Tile(i, TileType.Placeholder, position);			
		}
		this.FindNeighbours();
		return Tiles;
	}

	private void FindNeighbours() {
		for (int r = 0; r < this.Rows; r++) {
			for (int c = 0; c < this.Columns; c++) {
				Tile tile = this.Tiles[r * this.Columns + c];

				// Cannot have a bottom neighbour in the last row
				if (r < this.Rows - 1) {
                    tile.AddNeighbour(Sides.Bottom, Tiles[this.Columns * (r + 1) + c]);              
				}
				// Cannot have a right neighbour in the last column
				if (c < this.Columns - 1) {
                    tile.AddNeighbour(Sides.Right, Tiles[this.Columns * r + c + 1]);                   
                }
				// Cannot have a top neighbour in the first row
				if (r > 0) {
                    tile.AddNeighbour(Sides.Top, Tiles[this.Columns * (r - 1) + c]);                  
                }
				// Cannot have a left neighbour in the first column
				if (c > 0) {
                    tile.AddNeighbour(Sides.Left, Tiles[this.Columns * r + c - 1]);                 
                }
			}
		}
	}
}
