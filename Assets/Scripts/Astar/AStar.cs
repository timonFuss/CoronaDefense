using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AStar
{
    // Dictionary is more performant than a list!!
    private static Dictionary<Point, Node> nodes;

    private static void CreateNodes()
    {
        //Instantiates the dictionary
        nodes = new Dictionary<Point, Node>();

        //Run through all teh tiles in the game
        foreach (TileScript tile in LevelManager.Instance.Tiles.Values)
        {
            //Add the node to the dictionary
            nodes.Add(tile.GridPosition, new Node(tile));
        }
    }

    public static Stack<Node> GetPath(Point start, Point goal)
    {
        if(nodes == null)
        {
            CreateNodes();
        }

        HashSet<Node> openList = new HashSet<Node>();

        HashSet<Node> closedList = new HashSet<Node>();

        Stack<Node> finalPath = new Stack<Node>();


        Node currentNode = nodes[start];

        openList.Add(currentNode);

        while (openList.Count > 0)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Point neighbourPos = new Point(currentNode.GridPosition.X + x, currentNode.GridPosition.Y +  y);

                    if (LevelManager.Instance.InBounce(neighbourPos) && LevelManager.Instance.Tiles[neighbourPos].IsWalkable && neighbourPos != currentNode.GridPosition)
                    {
                        int gCost = 0;


                        //[14] [10] [14]
                        //[10] [S ] [10]
                        //[14] [10] [14]
                        if (Math.Abs(x-y) == 1)
                        {
                            gCost = 10;
                        }
                        else
                        {
                            gCost = 21;
                        }

                        Node neighbour = nodes[neighbourPos];



                        if (openList.Contains(neighbour))
                        {
                            if (currentNode.G + gCost < neighbour.G)
                            {
                                neighbour.CalcValues(currentNode, nodes[goal], gCost);
                            }
                        }
                        else if (!closedList.Contains(neighbour))
                        {
                            openList.Add(neighbour);
                            neighbour.CalcValues(currentNode, nodes[goal], gCost);
                        }
                    }   
                }
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (openList.Count > 0)
            {
                //Sorts the list by the F Value and selects the first on the list (lowest score)
                currentNode = openList.OrderBy(n => n.F).First();
            }

            if (currentNode == nodes[goal])
            {
                while (currentNode.GridPosition != start)
                {
                    finalPath.Push(currentNode);
                    currentNode = currentNode.Parent;
                }
                break;
            }

        }
        // ONLY FOR DEBUGGING
        //GameObject.Find("AStarDebugger").GetComponent<AStarDebugger>().DebugPath(openList, closedList, finalPath);

        foreach (Node node in finalPath)
        {
            //Debug.Log(node.GridPosition.X + ", " + node.GridPosition.Y);
            if (node.TileRef != nodes[start].TileRef && node.TileRef != nodes[goal].TileRef)
            {
                node.TileRef.SpriteRenderer.color = Color.blue;
            }
            
        }

        return finalPath;

    }
}
