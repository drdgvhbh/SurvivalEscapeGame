using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View {
    public int TileResolution;
    public Tile[] tiles;
    public MeshRenderer Renderer;
    public int Rows;
    public int Columns;
    public Texture2D[] TileTextures;

    public static Dictionary<TileType, Color[][]> TerrainColors = new Dictionary<TileType, Color[][]>();

    public View(int tileResolution, MeshBuilder mb, Texture2D[] tileTextures) {
        this.TileResolution = tileResolution;
        this.Renderer = mb.GetComponentInParent<MeshRenderer>();
        this.Rows = MeshBuilder.Rows;
        this.Columns = MeshBuilder.Columns;
        this.TileTextures = tileTextures;
        InitializeTerrainColors();
    }

    private void InitializeTerrainColors() {
        View.TerrainColors.Add(TileType.Placeholder, Global.GetColorsFromTexture2D(this.TileTextures[(int)TileType.Placeholder], this.TileResolution));
        View.TerrainColors.Add(TileType.Grass, Global.GetColorsFromTexture2D(this.TileTextures[(int)TileType.Grass], this.TileResolution));
        View.TerrainColors.Add(TileType.Sand, Global.GetColorsFromTexture2D(this.TileTextures[(int)TileType.Sand], this.TileResolution));
        View.TerrainColors.Add(TileType.Mountain, Global.GetColorsFromTexture2D(this.TileTextures[(int)TileType.Mountain], this.TileResolution));
        View.TerrainColors.Add(TileType.Water, Global.GetColorsFromTexture2D(this.TileTextures[(int)TileType.Water], this.TileResolution));
    }

    public void UpdateTileView() {
        Texture2D texture = new Texture2D(this.Columns * this.TileResolution, this.Rows * this.TileResolution);
        for (int y = 0; y < this.Rows; y++) {
            for (int x = 0; x < this.Columns; x++) {
                Tile tile = tiles[y * this.Columns + x];
                Color[] autoTile = View.TerrainColors[tile.Type][tile.AutoTileID];
                texture.SetPixels( 
                    x * this.TileResolution, 
                    this.Rows * this.TileResolution - y * this.TileResolution - this.TileResolution, 
                    this.TileResolution, 
                    this.TileResolution, 
                    autoTile
                );
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        this.Renderer.sharedMaterials[0].mainTexture = texture;

        Debug.Log("Done Texture!");
    }
}
