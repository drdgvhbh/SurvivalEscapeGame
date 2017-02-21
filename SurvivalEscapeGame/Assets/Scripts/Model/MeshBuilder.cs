using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class MeshBuilder : MonoBehaviour {
    [HideInInspector]
    public MeshFilter Filter;
    [HideInInspector]
    public MeshRenderer Renderer;
    [HideInInspector]
    public MeshCollider Collider;

	[Header("MeshBuilder Grid Dimensions")]
	public int Columns = 10;
	public int Rows = 10;
	public float TileSize = 1.0f;
	[HideInInspector]
    public int NumTiles;
	private Mesh GameGrid;

	private void Awake() {
		this.Filter = this.GetComponentInParent<MeshFilter>();
		this.Renderer = this.GetComponentInParent<MeshRenderer>();
		this.Collider = this.GetComponentInParent<MeshCollider>();
		this.NumTiles = this.Columns * this.Rows;
		this.GameGrid = this.BuildMesh();
	}

	public Mesh GetGameGrid() {
		return this.GameGrid;
	}

	public Mesh BuildMesh() {
		int vSizeX = this.Columns + 1;
		int vSizeY = this.Rows + 1;
		int numVerts = vSizeX * vSizeY;
		int numTris = this.NumTiles * Global.TrianglesInASquare;

		//Intialization
		Vector3[] verts = new Vector3[numVerts];
		Vector3[] norms = new Vector3[numVerts];
		Vector2[] uv = new Vector2[numVerts];
		int[] trisAsVerts = new int[numTris * Global.VertsInATri];

		for (int y = 0; y < vSizeY; y++) {
			for (int x = 0; x < vSizeX; x++) {
				verts[y * vSizeX + x] = new Vector3(x * this.TileSize, -y * this.TileSize, 0);
				norms[y * vSizeX + x] = Vector3.back;
				uv[y * vSizeX + x] = new Vector2((float)x / Columns, 1f - (float)y / Rows);
			}
		}

		for (int y = 0; y < this.Rows; y++) {
			for (int x = 0; x < this.Columns; x++) {
				int squareIndex = y * this.Columns + x;
				int triOffset = squareIndex * 6;
				trisAsVerts[triOffset + 0] = y * vSizeX + x;
				trisAsVerts[triOffset + 1] = y * vSizeX + vSizeX + x + 1;
				trisAsVerts[triOffset + 2] = y * vSizeX + vSizeX + x;

				trisAsVerts[triOffset + 3] = y * vSizeX + x + 1;
				trisAsVerts[triOffset + 4] = y * vSizeX + vSizeX + x + 1;
				trisAsVerts[triOffset + 5] = y * vSizeX + x;
			}
		}

		Debug.Log(this.GetComponentInParent<Transform>().name + " initialized.");

		Mesh mesh = new Mesh();
		mesh.vertices = verts;
		mesh.triangles = trisAsVerts;
		mesh.normals = norms;
		mesh.uv = uv;

		this.Filter.mesh = mesh;
		this.Collider.sharedMesh = mesh;

		return mesh;
	} 
}
