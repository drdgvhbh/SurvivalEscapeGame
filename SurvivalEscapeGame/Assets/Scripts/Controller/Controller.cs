using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller{
	private Model Model;
	private View View;
	public Controller(Model model) {
		this.Model = model;
	}

	public void SetView(View view) {
		this.View = view;
	}

    public void SetTileResolutionView(int resolution) {
        this.View.TileResolution = resolution;
    }

	public void SetTileView(Tile[] tile) {
        this.View.tiles = tile;
	}

    public void UpdateTileView() {
        this.View.UpdateTileView();
    }

}
