using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {
    private Tile Tile;
    private PathNode ParentNode;
    //Cost from CurrentTile to this Tile
    private int G;
    //Estimated Cost from this Tile to Destination
    private int H;
    //Total Cost
    private int F;

    public static int GetDistanceToNode(Tile start, Tile end, Grid grid ) {
        int currentRow = start.Index / grid.NumColumns;
        int currentColumn = start.Index % grid.NumColumns;
        int destRow = end.Index / grid.NumColumns;
        int destColumn = end.Index % grid.NumColumns;
        return (Mathf.Abs(destRow - currentRow) + Mathf.Abs(destColumn - currentColumn));
    }

    public static PathNode GetLowestNode(List<PathNode> list) {
        int currentLowest = int.MaxValue;
        PathNode lowest = new PathNode(null, null);
        foreach (PathNode pd in list) {
            if (pd.GetF() < currentLowest) {
                currentLowest = pd.GetF();
                lowest = pd;
            }
        }
        return lowest;
    }

    public static List<PathNode> GetTilesAsList(Dictionary<Tile.Sides, Tile> tiles, PathNode baseNode, bool IsWalkable, Tile destination, List<PathNode> NotInthisList, Grid gameGrid) {
        List<PathNode> pnTiles = new List<PathNode>();
        foreach (KeyValuePair<Tile.Sides, Tile> t in tiles) {
            Tile nTile = t.Value;
            if (nTile != null && (nTile.IsWalkable == IsWalkable) && !NotInthisList.Exists(x => x.GetTile() == nTile)) {
                PathNode pn = new PathNode(t.Value, baseNode);
                pnTiles.Add(pn);
                if (IsWalkable == true) {
                    pn.SetG(pn.GetParent().GetG() + pn.GetTile().MovementCost);
                    pn.SetH(GetDistanceToNode(pn.GetTile(), destination, gameGrid));
                    //Debug.Log(" NodeID: " + baseNode.GetTile().Id + ", LookingID: " + pn.GetTile().Id + ", G: " + pn.GetG() + ", H: " + pn.GetH() + ", F: " + pn.GetF());
                }
            }
        }
        return pnTiles;
    }


    public PathNode(Tile tile, PathNode parent, int g, int h) {
        SetTile(tile);
        SetParent(parent);
        SetG(g);
        SetH(h);
        CalculateF();
    }

    public PathNode(Tile tile, PathNode parent) : this(tile, parent, 0, 0) {
    }

    public Tile GetTile() {
        return Tile;
    }

    public void SetTile(Tile t) {
        Tile = t;
    }

    public int GetG() {
        return G;
    }

    public void SetG(int g) {
        G = g;
        CalculateF();
    }

    public int GetH() {
        return H;
    }

    public void SetH(int h) {
        H = h;
        CalculateF();
    }

    public PathNode GetParent() {
        return ParentNode;
    }

    public void SetParent(PathNode p) {
        this.ParentNode = p;
    }

    public int GetF() {
        return F;
    }

    private void CalculateF() {
        F = G + H;
    }
}
