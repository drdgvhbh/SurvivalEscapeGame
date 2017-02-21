using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour {
	private Mesh GameGrid;
	private MeshBuilder Mb;
	private TerrainBuilder Tb;
	private Controller Controller;
	private View View;
	private Tile[] Terrain;

    [Header("View Properties")]
    public int TileResolution = 18;
    public Texture2D[] TileTextures;

    private void Start() {
		this.Mb = this.GetComponentInChildren<MeshBuilder>();
		this.GameGrid = Mb.GetGameGrid();
		this.Tb = new TerrainBuilder(Mb.NumTiles, Mb.Rows, Mb.Columns);
		this.Terrain = Tb.CreateTerrain();
		this.Controller = new Controller(this);
		this.View = new View(this.TileResolution, this.Mb, TileTextures);
		this.Controller.SetView(this.View);
		this.Controller.SetTileView(this.Terrain);
        this.Controller.UpdateTileView();
	}
}
