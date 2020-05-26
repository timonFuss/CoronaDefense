using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{

    public Point GridPosition { get; private set; }
    public bool IsPlaceable { get; private set; }

    public bool IsWalkable { get; private set; }

    public SpriteRenderer SpriteRenderer{ get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(Point gridPos, Vector3 worldPos, Transform parent, bool placeable, bool walkable) 
    {
        this.GridPosition = gridPos;
        this.IsPlaceable = placeable;
        this.IsWalkable = walkable;
        transform.position = worldPos;
        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }

    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && this.IsPlaceable && GameManager.Instance.ClickedButton != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PlaceTower();
                this.IsWalkable = false;
            }
        } 
    }

    private void PlaceTower()
    {
        GameObject tower = (GameObject)Instantiate(GameManager.Instance.ClickedButton.TowerPrefab, transform.position, Quaternion.identity);
        tower.GetComponent<SpriteRenderer>().sortingOrder = GridPosition.Y;
        tower.transform.SetParent(transform);

        Hover.Instance.Deactivate();

        GameManager.Instance.BuyTower();
    }
}
