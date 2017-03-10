using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using SimpleJSON;
using System.IO;

public class TerrainBuilder {
    private int NumTiles;
    private int Rows;
    private int Columns;
    private float TileSize;
    private Tile[] Tiles;
    private Vector3[] Normals;
    private Mesh GameGrid;
    private JSONNode JsonNode;

    private static Dictionary<int, TileType> InverseTileType = new Dictionary<int, TileType>() {
        { -1, TileType.Empty},
        { 0, TileType.Placeholder},
        { 1, TileType.Grass },
        { 2, TileType.Sand },
        { 3, TileType.Mountain},
        { 4, TileType.Water}
    };

    private static Dictionary<TileType, string> TileTypeAsString = new Dictionary<TileType, string>() {
        { TileType.Empty, "Empty" },
        {  TileType.Placeholder, "Placeholder" },
        { TileType.Grass, "Grass"  },
        { TileType.Sand, "Sand" },
        { TileType.Mountain, "Mountain" },
        { TileType.Water, "Water" }
    };

    public static Dictionary<TileType, List<Item[]>> ItemLocations = new Dictionary<TileType, List<Item[]>>() {
        { TileType.Empty, new List<Item[]>() },
        { TileType.Placeholder, new List<Item[]>() },
        { TileType.Grass, new List<Item[]>() {
                {new Stick[0]},
                {new Stone[0]},
                {new Wood[0]},
                {new Berry[0]}
            }
        },
        { TileType.Sand, new List<Item[]>() {
                {new Gem[0]},
                {new Stick[0]},
                {new Stone[0]},
                {new Coconut[0]}
            }
        },
        { TileType.Mountain, new List<Item[]>()},
        { TileType.Water, new List<Item[]>()}
    };

    public static Dictionary<TileType, List<TileType>> BorderAllowances = new Dictionary<TileType, List<TileType>>() {
        { TileType.Grass, new List<TileType>()
            { TileType.Grass, TileType.Sand, TileType.Mountain}
        },
        { TileType.Sand, new List<TileType>()
            { TileType.Sand, TileType.Grass, TileType.Water}
        },
        { TileType.Mountain, new List<TileType>()
            { TileType.Mountain, TileType.Grass}
        },
        { TileType.Water, new List<TileType>()
            { TileType.Water, TileType.Sand}
        },
    };


    public TerrainBuilder(int numTiles, int rows, int columns, float tileSize, ref Vector3[] norms, Mesh gameGrid) {
        this.NumTiles = numTiles;
        this.Rows = rows;
        this.Columns = columns;
        this.TileSize = tileSize;
        this.Tiles = new Tile[this.NumTiles];
        this.Normals = norms;
        this.GameGrid = gameGrid;
        string path = Application.streamingAssetsPath + "/ItemDropChance.json";
        string jsonString = File.ReadAllText(path);
        JsonNode = JSON.Parse(jsonString);
    }


    public Tile[] CreateTerrain() {
        //Setup tiles randomly
        for (int i = 0; i < NumTiles; i++) {
            int currentRow = i / this.Columns;
            int currentColumn = i % this.Columns;
            Vector3 position = Global.Origin + Global.Offset + new Vector3(this.TileSize * currentColumn, -this.TileSize * currentRow);
            int[] normIdx = new int[Global.TrianglesInASquare * Global.VertsInATri];
            for (int j = 0; j < Global.TrianglesInASquare * Global.VertsInATri; j++) {
                normIdx[j] = this.GameGrid.triangles[j + i * Global.TrianglesInASquare * Global.VertsInATri];
            }
            if (currentRow == 0 || currentColumn == 0 || currentRow == this.Rows - 1 || currentColumn == this.Columns - 1) {
                this.Tiles[i] = new Tile(i, TileType.Sand, position, ref this.Normals, normIdx);
            } else {
                this.Tiles[i] = new Tile(i, TerrainBuilder.InverseTileType[Random.Range(1, 5)], position, ref this.Normals, normIdx);
            }
            //this.Tiles[i] = new Tile(i, CalculateTileType(i, currentRow, currentColumn), position, ref this.Normals, normIdx);

            /*Object prefab = Resources.Load("Prefabs/RandomText");
            GameObject tileNum = (GameObject)GameObject.Instantiate(prefab);
            tileNum.transform.SetParent(GameObject.Find("GridNumbers").transform);
            tileNum.transform.localPosition = (position - Global.SmallOffset) * 32 + new Vector3(160, -160);
            tileNum.GetComponent<Text>().text = i.ToString();
            */
        }
        this.FindNeighbours();
        //horrendous time complexity xd
        for (int m = 0; m < 4; m++) {
            //Fix any tile issues
            for (int i = 0; i < NumTiles; i++) {
                int currentRow = i / this.Columns;
                int currentColumn = i % this.Columns;
                if (!(currentRow == 0 || currentColumn == 0 || currentRow == this.Rows - 1 || currentColumn == this.Columns - 1)) {
                    TileType currentType = Tiles[i].Type;
                    Tile[] neighbours = Tiles[i].GetNeighbours();
                    List<TileType> neighbourTypes = new List<TileType>();
                    for (int j = 0; j < neighbours.Length; j++) {
                        neighbourTypes.Add(neighbours[j].Type);
                    }
                    bool change = false;
                    int iterations = 0;
                    do {
                        change = false;
                        foreach (TileType tt in neighbourTypes) {
                            if (!TerrainBuilder.BorderAllowances[tt].Contains(currentType) && change == false) {
                                Tiles[i].SetTileType(TerrainBuilder.BorderAllowances[tt][Random.Range(0, TerrainBuilder.BorderAllowances[tt].Count)]);
                                //Debug.Log("Id: " + i + ", OldType: " + currentType + ", NewType: " + Tiles[i].Type + ", CheckedN: " + tt);
                                currentType = Tiles[i].Type;
                                change = true;
                                break;
                            }
                        }
                        iterations++;
                        if (iterations > 5)
                            break;
                    } while (change == true && iterations <= 5);
                }
            }
        }
        foreach (Tile t in Tiles) {
            t.CalculateAutoTileID();
            var thisTextNode = JsonNode["ItemDropChance"][TerrainBuilder.TileTypeAsString[t.Type]];
            for (int i = 0; i < ItemLocations[t.Type].Count; i++) {
                var itemName = ItemLocations[t.Type][i].GetType().GetElementType();
                float rand = Random.Range(0.0f, 100.0f);
                if (rand <= thisTextNode[itemName.ToString()]) {
                    var obj = (Item)System.Activator.CreateInstance(
                         itemName, 
                         Item.IdCounter++, 
                         Random.Range(0, t.GetTileDepth()),
                         false,
                         1);
                    t.AddItem(obj);
                }
            }
        }
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
