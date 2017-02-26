﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour {
	private Mesh GameGrid;
	private MeshBuilder Mb;
	private TerrainBuilder Tb;
	private Controller Controller;
	private View View;
	private Tile[] Terrain;
    private bool Day;

    public int Key = 1;

    [Header("Prefabs")]
    public GameObject Player1;

    [Header("View Properties")]
    public int TileResolution = 32;
    public Texture2D[] TileTextures;

    private void Start() {
		this.Mb = this.GetComponentInChildren<MeshBuilder>();
		this.GameGrid = Mb.GetGameGrid();
        this.Day = true;
        Global.SmallOffset = new Vector3(-this.Mb.TileSize / 2.0f, this.Mb.TileSize / 2.0f);
        Global.Offset = Global.SmallOffset +
                        new Vector3((int)(-this.Mb.TileSize * this.Mb.Columns / 2.0f), (int)(this.Mb.TileSize * this.Mb.Rows / 2.0f));
        this.Mb.transform.localPosition = this.Mb.transform.localPosition + Global.Offset;
        this.Tb = new TerrainBuilder(Mb.NumTiles, Mb.Rows, Mb.Columns, this.Mb.TileSize, ref this.Mb.Norms, this.GameGrid);
		this.Terrain = Tb.CreateTerrain();
		this.Controller = new Controller(this);
		this.View = new View(this.TileResolution, this.Mb, TileTextures);
		this.Controller.SetView(this.View);
		this.Controller.SetTileView(this.Terrain);
        this.Controller.UpdateTileView();
        this.CreatePlayer();
	}

    private void CreatePlayer() {
        GameObject player = Instantiate(Player1, Terrain[0].GetPosition() - Global.SmallOffset, Quaternion.identity);
        player.transform.SetParent(this.GetComponent<Transform>());
        player.AddComponent<PlayerData>();
        player.GetComponent<PlayerData>().SetCurrentTile(Terrain[0]);
        player.GetComponent<PlayerInput>().SetPlayerData(player.GetComponent<PlayerData>());
    }

    public Mesh GetGameGrid() {
        return this.GameGrid;
    }

    public MeshBuilder GetMeshBuilder() {
        return this.Mb;
    }

    public bool IsDay() {
        return this.Day;
    }
}
