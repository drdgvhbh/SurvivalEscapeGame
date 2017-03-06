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
    private bool Day;

    public int Key = 1;

    [SerializeField]
    private GameObject Enemy;

    [SerializeField]
    private GameObject ActivePanel;
    [SerializeField]
    private GameObject ActiveSlot;
    private static int NumberOfActiveSlots = 6;
    public static List<GameObject> ActiveContainer = new List<GameObject>();

    [Header("Player")]
    public GameObject Player1;

    [Header("View Properties")]
    public int TileResolution = 32;
    public Texture2D[] TileTextures;

    private void Start() {
        CreateActivePanel(NumberOfActiveSlots);
        this.Mb = this.GetComponentInChildren<MeshBuilder>();
		this.GameGrid = Mb.GetGameGrid();
        this.Day = true;
        Global.SmallOffset = new Vector3(-this.Mb.TileSize / 2.0f, this.Mb.TileSize / 2.0f);
        Global.Offset = Global.SmallOffset +
                        new Vector3((int)(-this.Mb.TileSize * MeshBuilder.Columns / 2.0f), (int)(this.Mb.TileSize * MeshBuilder.Rows / 2.0f));
        this.Mb.transform.localPosition = this.Mb.transform.localPosition + Global.Offset;
        this.Tb = new TerrainBuilder(Mb.NumTiles, MeshBuilder.Rows, MeshBuilder.Columns, this.Mb.TileSize, ref this.Mb.Norms, this.GameGrid);
		this.Terrain = Tb.CreateTerrain();
		this.Controller = new Controller(this);
		this.View = new View(this.TileResolution, this.Mb, TileTextures);
		this.Controller.SetView(this.View);
		this.Controller.SetTileView(this.Terrain);
        this.Controller.UpdateTileView();
        this.CreatePlayerProperties();
        CreateEnemy();

    }

    private void CreateEnemy() {
        GameObject enemy = GameObject.Instantiate(Enemy);
        enemy.transform.SetParent(this.gameObject.transform);
        EnemyData ed = enemy.GetComponent<EnemyData>();
        ed.CurrentTile = Terrain[5];
        Terrain[5].CurrentGameObject = enemy;
        ed.DestinationTile = Player1.GetComponent<PlayerData>().CurrentTile;
        ed.Initialize(Player1);
    }

    private void CreatePlayerProperties() {
        Player1.transform.position = Terrain[0].GetPosition() - Global.SmallOffset;
        Player1.GetComponent<PlayerData>().SetCurrentTile(Terrain[0]);
        Terrain[0].CurrentGameObject = Player1;
        Player1.GetComponent<PlayerData>().LateStart();
        Player1.GetComponent<PlayerInput>().SetPlayerData(Player1.GetComponent<PlayerData>());
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

    public void CreateActivePanel() {
        ActiveContainer.Add(GameObject.Instantiate(ActiveSlot, ActivePanel.transform));       
    }
    public void CreateActivePanel(int quantity) {
        for (int i = 0; i < quantity; i++) {
            CreateActivePanel();
        }
    }
}
