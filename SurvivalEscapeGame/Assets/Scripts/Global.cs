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
	Empty = -1,
	Placeholder = 0,
	Grass = 1,
	Sand = 2,
    Mountain = 3,
    Water = 4
}

public enum PlayerActions {
	NotThisAction,
	Move,
	Dig,
	BuildTent,
    Attack
}

public enum ItemList {
    Placeholder,
	Shovel,
	Gem,
	Tent,
    Wood,
    Stick,
    Pickaxe,
    Stone
}

public static class Global {
	public const int TrianglesInASquare = 2;
	public const int VertsInATri = 3;
	public const int DepthVarianceMin = 0;
	public const int DepthVarianceMax = 3;
	public static Vector3 Origin = new Vector3(0, 0, 0);
	public static Vector3 SmallOffset = new Vector3(0, 0, 0);
	public static Vector3 Offset = new Vector3(0, 0, 0);
	public static UnityEngine.Random Random = new UnityEngine.Random();

	public static Dictionary<ItemList, string> ItemNames = new Dictionary<ItemList, string>() {
        {ItemList.Placeholder, "Placeholder" },
        {ItemList.Shovel, "Shovel" },
		{ItemList.Gem, "Gem" },
		{ItemList.Tent, "Tent" },
        {ItemList.Wood, "Wood" },
        {ItemList.Stick, "Stick" },
        {ItemList.Pickaxe, "PickAxe" },
        {ItemList.Stone, "Stone" }
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
