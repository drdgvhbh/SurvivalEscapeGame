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
    Grass = 1
}

public class Global {
	public const int TrianglesInASquare = 2;
	public const int VertsInATri = 3;

    public static Color[][] GetColorsFromTexture2D(Texture2D texture, int tileResolution) {
        int columns = texture.width / tileResolution;
        int rows = texture.height / tileResolution;
        Debug.Log("Columns " + columns + " Rows " + rows );

        Color[][] tiles = new Color[columns * rows][];

        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < columns; x++) {
                tiles[y * columns + x] = texture.GetPixels(x * tileResolution, y * tileResolution, tileResolution, tileResolution);
            }
        }
        return tiles;
    }
}
