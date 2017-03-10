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

    private IEnumerator Coroutine;

    private IEnumerator UpgradeEnemies(float waitTime) {
        while (true) {
            CreateEnemy(Random.Range(0, Mb.NumTiles));
            EnemyData.AttackDamage += 3;
            EnemyData.MovementSpeed += 0.11f;
            EnemyData.MaxHealth += 5;
            yield return new WaitForSeconds(waitTime);
        }
    }

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
        for (int i = 0; i < 8; i++) 
            CreateEnemy(Random.Range(0, Mb.NumTiles));
        new Radar(-1, false);
        new Cocoberry(-1, false);
        Coroutine = UpgradeEnemies(18.0f);
        StartCoroutine(Coroutine);
        Time.timeScale = 0;
    }

    private void CreateEnemy(int idx) {
            GameObject enemy = GameObject.Instantiate(Enemy);
            enemy.transform.SetParent(this.gameObject.transform);
            EnemyData ed = enemy.GetComponent<EnemyData>();
            ed.CurrentTile = Terrain[idx];
            Terrain[idx].CurrentGameObject = enemy;
            ed.DestinationTile = Player1.GetComponent<PlayerData>().CurrentTile;
            ed.Initialize(Player1);
    }

    private void CreatePlayerProperties() {
        bool walkable = false;
        int idx = 0;
        do {
            idx = Random.Range(0, Mb.NumTiles);
            if (Terrain[idx].IsWalkable == true) {
                walkable = true;
            }
        } while (walkable == false);

        Player1.transform.position = Terrain[idx].GetPosition() - Global.SmallOffset;
        Player1.GetComponent<PlayerData>().SetCurrentTile(Terrain[idx]);
        Terrain[idx].CurrentGameObject = Player1;
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
        GameObject activeSlot = GameObject.Instantiate(ActiveSlot);
        activeSlot.transform.SetParent(ActivePanel.transform, false);
        ActiveContainer.Add(activeSlot);       
    }
    public void CreateActivePanel(int quantity) {
        for (int i = 0; i < quantity; i++) {
            CreateActivePanel();
        }
    }
}
