using System;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    // I am going for a very simple map where just height and width are changeable
    // to at least have two points adapting the map feeling and flow
    // that already makes a little difference in the playstyle
    private int m_width = 20;
    private int m_height = 5;

    [SerializeField]
    private Camera m_camera;
    [SerializeField]
    private int m_wood;
    [SerializeField]
    private Text m_woodText;
    [SerializeField]
    private int m_gold;
    [SerializeField]
    private Text m_goldText;
    [SerializeField]
    private Transform m_map;
    [SerializeField]
    private Transform m_placeables;

    [Header("Controller")]
    [SerializeField]
    private EnemyController m_enemyController;
    [SerializeField]
    private UnitController m_unitController;

    [Header("Tiles")]
    [SerializeField]
    private TileSO m_defaultTile;
    [SerializeField]
    private BuildingSO m_ressourceTile;
    [SerializeField]
    private BuildingSO m_baseTile;

    [Header("Units")]
    [SerializeField]
    private UnitSO m_workerUnit;

    [Header("Buttons")]
    [SerializeField]
    private Button m_buyWorkerButton;
    [SerializeField]
    private Button m_buildBarracksButton;
    [SerializeField]
    private Button m_buildTreeButton;
    [SerializeField]
    private Button m_upgradeUnitsDamage;
    [SerializeField]
    private Button m_upgradeUnitsRange;
    [SerializeField]
    private Button m_upgradeUnitsSpeed;

    [SerializeField]
    private Text m_gameWonText;
    [SerializeField]
    private Text m_gameLostText;

    private bool m_buildTreeMode;
    private bool m_buildBarracksMode;

    private Grid m_grid;
    private Vector2 m_center;

    [SerializeField]
    private AbilitySO m_basicUnitAbility;

    public void Start()
    {
        SetMapSize();
        m_grid = CreateSimpleMap();
        VisualizeMap();
        CenterCamera();

        PlaceBases();
        PlaceRessources();

        m_unitController.Inject(m_grid, m_defaultTile.Asset.bounds.size);
        m_enemyController.Inject(m_grid, m_unitController, m_workerUnit, new Vector2Int(m_width - 3, m_height / 2));
        m_enemyController.OnPlaceBarracks += M_enemyController_OnPlaceBarracks;

        m_woodText.text = m_wood.ToString();
        m_goldText.text = m_gold.ToString();

        m_unitController.RessourcesEarned += M_unitController_RessourcesEarned;

        m_buyWorkerButton.onClick.AddListener(OnBuyWorkerButton);
        m_buildTreeButton.onClick.AddListener(OnBuildTreeButton);
        m_buildBarracksButton.onClick.AddListener(OnBuildBarracksButtonPressed);

        m_upgradeUnitsDamage.onClick.AddListener(OnUpgradeUnitsDamagePressed);
        m_upgradeUnitsRange.onClick.AddListener(OnUpgradeUnitsRangePressed);
        m_upgradeUnitsSpeed.onClick.AddListener(OnUpgradeUnitsSpeedPressed);
    }

    private void SetMapSize()
    {
        float aspect = (float)Screen.width / Screen.height;
        float worldHeight = m_camera.orthographicSize * 2;
        float worldWidth = worldHeight * aspect;
        Bounds bounds = m_defaultTile.Asset.bounds;
        m_width = (int)Mathf.Ceil(worldWidth / bounds.size.x);
        m_height = (int)Mathf.Ceil(worldHeight / bounds.size.y);
    }

    private Grid CreateSimpleMap()
    {
        Grid grid = new Grid(m_defaultTile, m_height, m_width);
        return grid;
    }

    private void VisualizeMap()
    {
        for(int x = 0; x < m_width; x++)
        {
            for(int y = 0; y < m_height; y++)
            {
                Tile tile = TileFactory.getTile(m_defaultTile);
                tile.Pos = new Vector2Int(x, y);
                tile.OnMousePressedOnTile += Tile_OnMousePressedOnTile;
                PlaceGo(tile.gameObject, x, y);

                m_grid.SetTileForCoords(x, y, tile);

                if (x == m_width / 2 && y == m_height / 2) m_center = tile.gameObject.transform.position;
            }
        }
    }

    private void Tile_OnMousePressedOnTile(Tile tile)
    {
        if(m_grid.GetPlaceable(tile.Pos.x, tile.Pos.y) == null)
        {
            if(m_buildTreeMode)
            {
                PlacePlayerTree(tile.Pos.x, tile.Pos.y);
                m_buildTreeMode = false;

                m_wood -= 1;
                m_woodText.text = m_wood.ToString();

                ResetButtons();
            }
            else if(m_buildBarracksMode)
            {
                PlaceBarracks(tile.Pos.x, tile.Pos.y, PlayerType.Player);
                m_buildBarracksMode = false;

                m_wood -= 100;
                m_woodText.text = m_wood.ToString();

                ResetButtons();
            }
        }
    }

    // bases will be placed straight forward in one tile distance from the left and right border
    // and in the middle of the y-axis
    private void PlaceBases()
    {
        int y = m_height / 2;

        PlaceBarracks(2, y, PlayerType.Player);
        PlaceBarracks(m_width - 3, y, PlayerType.Enemy);
    }

    private void PlaceBarracks(int x, int y, PlayerType player)
    {
        Tile tile = TileFactory.getBuilding(m_baseTile, player);
        PlaceGo(tile.gameObject, x, y);
        Building building1 = tile as Building;
        building1.BuildingDestroyed += Building1_BuildingDestroyed;
        building1.Player = player;
        m_grid.SetPlaceableForCoords(x, y, building1);

        for (int i = 0; i < building1.UnitSpawners.Count; i++)
        {
            building1.UnitSpawners[i].Inject(m_unitController);
            building1.UnitSpawners[i].SpawnPosition = new Vector2Int(x, y);
            building1.UnitSpawners[i].PlayerType = player;
            building1.UnitSpawners[i].Active = true;
        }
    }

    // as the enemy will only have one building for now. Win conditions is really simple
    // if one building of the enemy is destroyed, the game is won
    // as soon as there is a simple AI or more enemy buildings that needs to change
    private void Building1_BuildingDestroyed(Building building, Unit byUnit)
    {
        Destroy(building.gameObject);
        if (m_grid.GetEnemyBuilding(PlayerType.Enemy) == null)
        {
            m_gameWonText.gameObject.SetActive(true);
        }
        else if(m_grid.GetEnemyBuilding(PlayerType.Player) == null)
        {
            m_gameLostText.gameObject.SetActive(true);
        }
    }

    // ressources will be placed left and right on the border (its placed here so I can better play around with the placement and maybe change it)
    private void PlaceRessources()
    {
        for (int y = 0; y < m_height; y++)
        {
            PlacePlayerTree(0, y);

            Tile tile2 = TileFactory.getBuilding(m_ressourceTile, PlayerType.Enemy);
            PlaceGo(tile2.gameObject, m_width - 1, y);
            Building building2 = (Building)tile2;
            building2.BuildingDestroyed += Building_BuildingDestroyed;
            m_grid.SetPlaceableForCoords(m_width - 1, y, building2);
        }
    }

    private void PlacePlayerTree(int x, int y)
    {
        Tile tile = TileFactory.getBuilding(m_ressourceTile, PlayerType.Player);
        PlaceGo(tile.gameObject, x, y);
        Building building = (Building)tile;
        building.BuildingDestroyed += Building_BuildingDestroyed;
        m_grid.SetPlaceableForCoords(x, y, building);

    }

    private void Building_BuildingDestroyed(Building building, Unit byUnit)
    {
        Destroy(building.gameObject);
    }

    private void PlaceGo(GameObject go, int x, int y)
    {
        go.name = string.Format("{0} / {1}", x.ToString("d2"), y.ToString("d2"));
        go.transform.parent = m_placeables;
         
        Bounds bounds = go.GetComponent<SpriteRenderer>().bounds;
        go.transform.localPosition = new Vector2(x * bounds.size.x, y * bounds.size.y);
    }

    private void CenterCamera()
    {
        m_camera.transform.position = new Vector3(m_center.x, m_center.y, m_camera.transform.position.z);
    }

    private void OnBuyWorkerButton()
    {
        if (m_gold < 10)
        {
            m_buyWorkerButton.GetComponent<Image>().color = Color.red;
            Invoke("ResetButtons", 1);
            return;
        }

        m_gold -= 10;
        m_goldText.text = m_gold.ToString();

        m_unitController.SpawnWorkerUnit(PlayerType.Player, m_workerUnit, new Vector2Int(2, m_height / 2));
    }

    private void OnBuildBarracksButtonPressed()
    {
        if (m_wood < 100)
        {
            m_buildBarracksButton.GetComponent<Image>().color = Color.red;
            Invoke("ResetButtons", 1);
            return;
        }

        if (m_buildBarracksMode)
        {
            m_buildBarracksMode = false;
        }
        else
        {
            m_buildBarracksButton.GetComponent<Image>().color = Color.green;
            m_buildBarracksMode = true;
        }
    }

    private void OnBuildTreeButton()
    {
        if (m_wood < 1)
        {
            m_buildTreeButton.GetComponent<Image>().color = Color.red;
            Invoke("ResetButtons", 1);
            return;
        }

        if (m_buildTreeMode)
        {
            m_buildTreeMode = false;
        }
        else
        {
            m_buildTreeButton.GetComponent<Image>().color = Color.green;
            m_buildTreeMode = true;
        }
    }

    private void ResetButtons()
    {
        m_buildBarracksButton.GetComponent<Image>().color = Color.white;
        m_buildTreeButton.GetComponent<Image>().color = Color.white;
        m_buyWorkerButton.GetComponent<Image>().color = Color.white;
        m_upgradeUnitsSpeed.GetComponent<Image>().color = Color.white;
        m_upgradeUnitsRange.GetComponent<Image>().color = Color.white;
        m_upgradeUnitsDamage.GetComponent<Image>().color = Color.white;

    }


    private void OnUpgradeUnitsSpeedPressed()
    {
        if (m_gold < 100)
        {
            m_upgradeUnitsSpeed.GetComponent<Image>().color = Color.red;
            Invoke("ResetButtons", 1);
            return;
        }

        m_basicUnitAbility.Speed += 1;
        m_gold -= 100;
        m_goldText.text = m_gold.ToString();
    }

    private void OnUpgradeUnitsRangePressed()
    {
        if (m_gold < 100)
        {
            m_upgradeUnitsRange.GetComponent<Image>().color = Color.red;
            Invoke("ResetButtons", 1);
            return;
        }

        m_basicUnitAbility.Range += 1;
        m_gold -= 100;
        m_goldText.text = m_gold.ToString();
    }

    private void OnUpgradeUnitsDamagePressed()
    {
        if (m_gold < 100)
        {
            m_upgradeUnitsDamage.GetComponent<Image>().color = Color.red;
            Invoke("ResetButtons", 1);
            return;
        }

        m_basicUnitAbility.Damage += 1;
        m_gold -= 100;
        m_goldText.text = m_gold.ToString();
    }

    private void M_enemyController_OnPlaceBarracks(int x, int y)
    {
        PlaceBarracks(x, y, PlayerType.Enemy);
    }

    private void M_unitController_RessourcesEarned(int wood, int gold)
    {
        m_gold += gold;
        m_wood += wood;

        m_goldText.text = m_gold.ToString();
        m_woodText.text = m_wood.ToString();
    }
}