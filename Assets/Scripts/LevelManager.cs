using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager>
{

    [SerializeField]
    private GameObject[] tilePrefabs;
    [SerializeField]
    private CameraMovement cameraMovement;

    [SerializeField]
    private Transform map;

    [SerializeField]
    private TextAsset jsonFile;

    public Dictionary<Point, TileScript> Tiles { get; set; }

    private Point mapSize;

    public TileScript startTile, finishTile;

    public SpawnTile StartSpawnTile;

    private Stack<Node> path;

    private LevelAttributes level;

    public int LevelType
    {
        get { return level.levelType; }
    }

    public int LevelIdx
    {
        get { return level.levelIdx;  }
    }

    public int Waves
    {
        get { return level.waves; }
    }

    int yLength;
    int xLength;

    public Stack<Node> Path
    {
        get
        {
            // TODO: Überhaupt benötigt?
            /*
            if(path == null)
            {
                GeneratePath();
            }
            */
            return new Stack<Node>(new Stack<Node>(path));
        }
    }


    public float TileSize
    {
        get 
        {
            return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CreateLevel();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// Creates new level
    private void CreateLevel()
    {
        Tiles = new Dictionary<Point, TileScript>();

        Vector3 maxTile = Vector3.zero;

        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));


        
        level = JsonUtility.FromJson<LevelAttributes>(jsonFile.text);
        GameManager.Instance.RemainingWaves = level.waves;
        xLength = level.tiles.Length / level.rows;
        yLength = level.rows;
        int idx = 0;

        for (int y = 0; y < level.rows; y++)
        {
            for (int x = 0; x < xLength; x++)
            {
                PlaceTile(level.tiles[idx], x, y, worldStart);
                idx += 1;
            }
        }

        if(level.levelType == 1 || level.levelType == 3)
        {
            GeneratePath();
        }
        else
        {
            Debug.Log("Freestyle path --> erst bei wavespawn berechnen");
        }
        

        maxTile = Tiles[new Point(xLength - 1, level.rows - 1)].transform.position;

        cameraMovement.SetLimits(new Vector3(maxTile.x + TileSize, maxTile.y - TileSize));
        

        /*
        for (int y = 0; y < mapY; y++)
        {

            char[] newTiles = mapData[y].ToCharArray();

            for (int x = 0; x < mapX; x++)
            {
                PlaceTile(newTiles[x].ToString(), x, y, worldStart);
            }
        }
        maxTile = Tiles[new Point(mapX - 1, mapY - 1)].transform.position;

        cameraMovement.SetLimits(new Vector3(maxTile.x + TileSize, maxTile.y - TileSize));
        */


    }

    
    private void PlaceTile(Tile tile, int x, int y, Vector3 worldStart)
    {

        TileScript newTile = Instantiate(tilePrefabs[tile.assetID]).GetComponent<TileScript>();
        newTile.Setup(new Point(x, y), new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0), map, tile.placeable, tile.walkable, tile.start, tile.finish);

        if (newTile.IsStart == true)
        {
            startTile = newTile;
            //startTile.SpriteRenderer.color = Color.green;
            //StartSpawnTile.SpawningTile = startTile;
        }else if(newTile.IsFinish == true)
        {
            finishTile = newTile;
            //finishTile.SpriteRenderer.color = Color.red;
            finishTile.tag = "DespawnTag";
        }
    }
    

    /*
    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {

        int tileIndex = int.Parse(tileType);

        TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();
        newTile.Setup(new Point(x, y), new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0), map, tileIndex == 1, tileIndex == 3);
    }
    */

    public bool InBounce(Point position)
    {
        return position.X >= 0 && position.Y >= 0 && position.X < xLength && position.Y < yLength;
    }

    public void GeneratePath()
    {
        path = AStar.GetPath(startTile.GridPosition, finishTile.GridPosition);
    }

    [Serializable]
    private class Tile
    {
        public int assetID;
        public bool start;
        public bool finish;
        public bool placeable;
        public bool walkable;
    }

    [Serializable]
    private class LevelAttributes
    {
        public int levelIdx;
        public int rows;
        public int levelType;
        public int waves;
        public Tile[] tiles;
    }
}
