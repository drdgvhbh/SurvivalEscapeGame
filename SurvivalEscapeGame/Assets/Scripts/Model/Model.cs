using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour {
	private Mesh GameGrid;
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

    [SerializeField]
    private GameObject GameGridObj;
    private Grid Mb;

    [SerializeField]
    private GameObject InventoryPanel;
    [SerializeField]
    private GameObject StructurePanel;

    private bool init;


    private IEnumerator Coroutine;

    private IEnumerator UpgradeEnemies(float waitTime) {
        while (true) {
            CreateEnemy(Random.Range(0, Mb.NumTiles));
            CreateEnemy(Random.Range(0, Mb.NumTiles));
            EnemyData.AttackDamage += 3;
            EnemyData.MovementSpeed += 0.11f;
            EnemyData.MaxHealth += 5;
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void Awake() {
        init = false;
    }

    protected void Update() {
        if (init) {
            if (Player1.GetComponent<PlayerData>().Alive && (InventoryPanel.activeSelf || StructurePanel.activeSelf)) {
                Time.timeScale = 0;
            } else if ((Player1.GetComponent<PlayerData>().Alive && !(InventoryPanel.activeSelf || StructurePanel.activeSelf))) {
                Time.timeScale = 1.0f;
            } else {
                Time.timeScale = 0;
            }
        }
    }
    private void Start() {
        CreateActivePanel(NumberOfActiveSlots);
        Mb = GameGridObj.GetComponent<Grid>();
        this.GameGrid = GameGridObj.GetComponent<Mesh>();
        this.Day = true;
        Terrain = GameGridObj.GetComponent<TerrainData>().Tiles;
        this.CreatePlayerProperties();
        for (int i = 0; i < 1; i++) 
            CreateEnemy(Random.Range(0, Mb.NumTiles));
        new Radar(-1, false);
        new Cocoberry(-1, false);
        new Tent(-1, false);
        new Torch(-1, false);
        new Pickaxe(-1, false);
        new Wall(-1, false);
        new Spear(-1, false);
        new Granary(-1, false);
       
        Coroutine = UpgradeEnemies(15.0f);
        StartCoroutine(Coroutine);
        Time.timeScale = 0;
        Player1.GetComponent<PlayerData>().DiscoverTiles();
        init = true;
    }

    private void CreateEnemy(int idx) {
            GameObject enemy = GameObject.Instantiate(Enemy);
            enemy.transform.SetParent(this.gameObject.transform);
            EnemyData ed = enemy.GetComponent<EnemyData>();
            ed.CurrentTile = Terrain[idx];
            Terrain[idx].CurrentGameObject = enemy;
            ed.DestinationTile = Player1.GetComponent<PlayerData>().CurrentTile;
            ed.Initialize(Player1, Mb);
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

        Player1.transform.position = Terrain[idx].Position;
        Player1.GetComponent<PlayerData>().SetCurrentTile(Terrain[idx]);
        Terrain[idx].CurrentGameObject = Player1;
        Player1.GetComponent<PlayerData>().LateStart();
        Player1.GetComponent<PlayerInput>().SetPlayerData(Player1.GetComponent<PlayerData>());
        Player1.GetComponent<PlayerFogOfWar>().UpdateFogOfWar();
    }

    public Mesh GetGameGrid() {
        return this.GameGrid;
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
