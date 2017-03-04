using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sides {
	Top,
	Left,
	Right,
	Bottom
}

public enum TileType {
    Empty = -1,
    Placeholder = 0,
    Grass = 1,
    Sand = 2
}

public enum PlayerActions {
    Move,
    Dig
}

public enum ItemList {
    Shovel,
    Gem,
    Tent
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
        {ItemList.Shovel, "Shovel" },
        {ItemList.Gem, "Gem" },
        {ItemList.Tent, "Tent" }
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
