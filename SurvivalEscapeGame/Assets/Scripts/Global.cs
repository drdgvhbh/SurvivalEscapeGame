using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sides {
	Top = 0,
	Left = 1,
	Right = 2,
	Bottom = 3
}

public enum TileType {
	Grass = 0,
	Sand = 1,
    Mountain = 2,
    Water = 3,
    Vine = 4
}

public enum PlayerActions {
	NotThisAction,
	Move,
	Dig,
	BuildTent,
    Attack,
    Eat,
    BuildGranary,
    BuildWall,
    UseSpear,
    UseRadar,
    UseBeacon
}

public enum ItemList {
	Shovel,
	Gem,
	Tent,
    Wood,
    Stone,
    Spear,
    Coconut,
    Fabric,
    Radar,
    Berry,
    Cocoberry,
    Granary,
    Charcoal,
    Torch,
    Wall,
    SilverOre,
    Banana,
    SacredItem,
    DistressBeacon
}

public static class Global {
	public const int TrianglesInASquare = 2;
	public const int VertsInATri = 3;
	public const int DepthVarianceMin = -1;
	public const int DepthVarianceMax = 2;
	public static UnityEngine.Random Random = new UnityEngine.Random();

	public static Dictionary<ItemList, string> ItemNames = new Dictionary<ItemList, string>() {
        {ItemList.Shovel, "Shovel" },
		{ItemList.Gem, "Gem" },
		{ItemList.Tent, "Tent" },
        {ItemList.Wood, "Wood" },
        {ItemList.Stone, "Stone" },
        {ItemList.Spear, "Spear" },
        {ItemList.Coconut, "Coconut" },
        {ItemList.Fabric, "Fabric" },
        {ItemList.Radar, "Radar" },
        {ItemList.Berry, "Berry" },
        {ItemList.Cocoberry, "Cocoberry" },
        {ItemList.Granary, "Granary" },
        {ItemList.Charcoal, "Charcoal" },
        {ItemList.Torch, "Torch" },
        {ItemList.Wall, "Wall" },
        {ItemList.SilverOre, "SilverOre" },
        {ItemList.Banana, "Banana" },
        {ItemList.SacredItem, "SacredItem" },
        {ItemList.DistressBeacon, "DistressBeacon" }
    };


	public static Color[][] GetColorsFromTexture2D(Texture2D texture, int tileResolution) {
		int columns = texture.width / tileResolution;
		int rows = texture.height / tileResolution;

		Color[][] tiles = new Color[columns * rows][];

		for (int y = 0; y < rows; y++) {
			for (int x = 0; x < columns; x++) {
				tiles[y * columns + x] = texture.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
			}
		}
		return tiles;
	}
}
