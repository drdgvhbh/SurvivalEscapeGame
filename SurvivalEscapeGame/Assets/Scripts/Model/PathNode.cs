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

    public static List<PathNode> GetTilesAsList(Tile[] tiles, PathNode baseNode, bool IsWalkable, Tile destination, List<PathNode> NotInthisList) {
        List<PathNode> pnTiles = new List<PathNode>();
        for (int i = 0; i < tiles.Length; i++) {
            Tile nTile = tiles[i];
            if (nTile != null && (nTile.IsWalkable == IsWalkable) && !NotInthisList.Exists(x => x.GetTile() == nTile)) {
                PathNode pn = new PathNode(tiles[i], baseNode);
                pnTiles.Add(pn);
                if (IsWalkable == true) {
                    pn.SetG(pn.GetParent().GetG() + pn.GetTile().MovementCost);
                    int currentRow = pn.GetTile().Id / MeshBuilder.Columns;
                    int currentColumn = pn.GetTile().Id % MeshBuilder.Columns;
                    int destRow = destination.Id / MeshBuilder.Columns;
                    int destColumn = destination.Id % MeshBuilder.Columns;
                    pn.SetH(Mathf.Abs(destRow - currentRow + destColumn - currentColumn));
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
