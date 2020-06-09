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

    public Stack<Node> Path
    {
        get
        {
            if(path == null)
            {
                GeneratePath();
            }
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

        string[] mapData = ReadLevelText();

        mapSize = new Point(mapData[0].ToCharArray().Length, mapData.Length);

        int mapX = mapData[0].ToCharArray().Length;
        int mapY = mapData.Length;

        Vector3 maxTile = Vector3.zero;

        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));


        
        LevelTiles tilesInJson = JsonUtility.FromJson<LevelTiles>(jsonFile.text);
        int xLength = tilesInJson.tiles.Length / tilesInJson.rows;
        int idx = 0;

        for (int y = 0; y < tilesInJson.rows; y++)
        {
            for (int x = 0; x < xLength; x++)
            {
                PlaceTile(tilesInJson.tiles[idx], x, y, worldStart);
                idx += 1;
            }
        }

        AStar.GetPath(startTile.GridPosition, finishTile.GridPosition);

        maxTile = Tiles[new Point(xLength - 1, tilesInJson.rows - 1)].transform.position;

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

        if(newTile.IsStart == true)
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

    private string[] ReadLevelText() 
    {
        TextAsset bindData = Resources.Load("Level_1") as TextAsset;

        string data = bindData.text.Replace(Environment.NewLine, string.Empty);

        return data.Split('-');
    }

    public bool InBounce(Point position)
    {
        return position.X >= 0 && position.Y >= 0 && position.X <= mapSize.X && position.Y <= mapSize.Y;
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
    private class LevelTiles
    {
        public int rows;
        public Tile[] tiles;
    }
}
