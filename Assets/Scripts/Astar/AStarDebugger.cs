using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarDebugger : MonoBehaviour
{
    private TileScript start, goal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ClickTile();
    }

    private void ClickTile()
    {
        if (Input.GetMouseButtonDown(1))    //1 --> rightlick
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if(hit.collider != null)
            {
                TileScript tmp = hit.collider.GetComponent<TileScript>();

                if(tmp != null && tmp.IsWalkable)
                {
                    if(start == null)
                    {
                        start = tmp;
                        start.SpriteRenderer.color = Color.green;
                    }
                    else if(goal == null)
                    {
                        goal = tmp;
                        goal.SpriteRenderer.color = Color.red;

                        //TODO: Call this method after the map is instantiated 
                        AStar.GetPath(start.GridPosition, goal.GridPosition);
                    }
                }
            }
        }
    }

    public void DebugPath(HashSet<Node> openList, HashSet<Node> closedList, Stack<Node> finalPath)
    {
        /*
        foreach (Node node in openList)
        {
            if (node.TileRef != start && node.TileRef != goal)
            {
                node.TileRef.SpriteRenderer.color = Color.cyan;
            }
        }

        foreach (Node node in closedList)
        {
            if (node.TileRef != start && node.TileRef != goal)
            {
                node.TileRef.SpriteRenderer.color = Color.blue;
            }
        }
        */
        foreach (Node node in finalPath)
        {
            if (node.TileRef != start && node.TileRef != goal)
            {
                node.TileRef.SpriteRenderer.color = Color.blue;
            }
            
        }
    }


}
