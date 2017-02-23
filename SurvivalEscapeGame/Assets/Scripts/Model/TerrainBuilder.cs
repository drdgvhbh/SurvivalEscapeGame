using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainBuilder {
	private int NumTiles;
	private int Rows;
	private int Columns;
    private float TileSize;
	private Tile[] Tiles;

    public static Dictionary<ItemList, float> ItemChance = new Dictionary<ItemList, float>() {
        { ItemList.Gem, 100.0f}
    };

    public static Dictionary<TileType, Item[][]> ItemLocations = new Dictionary<TileType, Item[][]>() {
        { TileType.Empty, new Item[0][] },
        { TileType.Placeholder, new Item[][] {new Gem[0] }},
        { TileType.Grass, new Item[0][]}
    };

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
            int currentColumn = i % this.Columns;
            Vector3 position = Global.Origin + Global.Offset + new Vector3(this.TileSize * currentColumn, -this.TileSize * currentRow);
			this.Tiles[i] = new Tile(i, TileType.Placeholder, position);
            if (Random.Range(0.0f, 100.0f) <= ItemChance[ItemList.Gem]) {
                this.Tiles[i].AddItem(new Gem(
                                                Item.IdCounter, 
                                                Random.Range(0, this.Tiles[i].GetTileDepth()),
                                                false,
                                                1
                                                )
                                     );
                Item.IdCounter++;
            }
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
