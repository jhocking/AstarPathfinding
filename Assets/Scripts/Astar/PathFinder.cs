using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Astar
{
    public class PathFinder
    {
        private int finX;
        private int finY;
        private bool only4way; // 4 or 8 way movement?

        // a 2d array of different values representing map tiles
		// and the values that can be part of the path (ie. are walkable)
        private int[,] levelData;
        private int[] walkableValues;

        private Dictionary<string, PathNode> openList;
        private Dictionary<string, PathNode> closedList;

        // a list of coordinates that's the shortest path
        public List<Vector2Int> Path { get; private set; }

        public PathFinder(int xIni, int yIni, int xFin, int yFin, bool do4way, int[,] lvlData, int[] walkable = null)
        {
            finX = xFin;
            finY = yFin;
            only4way = do4way;
            levelData = lvlData;

            walkableValues = walkable;
            if (walkableValues == null)
            {
                walkableValues = new int[] { 0 }; // default in case it hasn't been set
            }

            openList = new Dictionary<string, PathNode>();
            closedList = new Dictionary<string, PathNode>();
            Path = new List<Vector2Int>();

            // first node is the starting point
            var node = new PathNode(xIni, yIni, 0, 0, null);
            openList[$"{xIni} {yIni}"] = node;

            SearchLevel();
        }

        // the pathfinding algorithm
        private void SearchLevel()
        {
            PathNode curNode = null;
            PathNode endNode = null;
            float lowF = Mathf.Infinity;

            // first determine node with lowest F
            foreach (var node in openList.Values)
            {
                var curF = node.g + node.h;

                // currently this is just a brute force loop through every item in the list
                // could be sped up using a sorted list or binary heap
                if (lowF > curF)
                {
                    lowF = curF;
                    curNode = node;
                }
            }

            // no path exists!
            if (curNode == null) { return; }

            // move selected node from open to closed list
            var label = $"{curNode.x} {curNode.y}";
            openList.Remove(label);
            closedList[label] = curNode;

            // check target
            if (curNode.x == finX && curNode.y == finY)
            {
                endNode = curNode;
            }

            // check each of the adjacent squares
            for (var i = -1; i < 2; i++)
            {
                for (var j = -1; j < 2; j++)
                {
                    var col = curNode.x + i;
                    var row = curNode.y + j;

                    // make sure is a neighboring (not current) node and is on the grid
                    // https://stackoverflow.com/questions/9404683/how-to-get-the-length-of-row-column-of-multidimensional-array-in-c
                    bool isNeighbor = only4way ? (i == 0 && j != 0) || (i != 0 && j == 0) : (i != 0 || j != 0); // 4 or 8 way movement?
                    if (isNeighbor && (col >= 0 && col < levelData.GetLength(0)) && (row >= 0 && row < levelData.GetLength(1)))
                    {
                        var key = $"{col} {row}";

                        // if walkable, not on closed list, and not already on open list - add to open list
                        // https://www.geeksforgeeks.org/c-sharp-check-if-an-array-contain-the-elements-that-match-the-specified-conditions/
                        if (Array.Exists(walkableValues, e => e == levelData[col, row]) && !closedList.ContainsKey(key) && !openList.ContainsKey(key))
                        {

                            // diagonals have greater movement cost
                            var g = 10;
                            if (i != 0 && j != 0)
                            {
                                g = 14;
                            }

                            // calculate heuristic value
                            var h = (col - finX) * (col - finX) + (row - finY) * (row - finY);
                            // TODO slightly different path results from:
                            //var h = (Mathf.Abs(col - finX)) + (Mathf.Abs(row - finY)) * 10;

                            var found = new PathNode(col, row, g, h, curNode);
                            openList[key] = found;
                        }
                    }
                }
            }

            // recurse if target not reached
            if (endNode == null)
            {
                SearchLevel();
            }
            else
            {
                RetracePath(endNode);
            }
        }
        
        // construct a list of points by retracing searched nodes
        private void RetracePath(PathNode node)
        {
            var step = new Vector2Int(node.x, node.y);
            Path.Add(step);

            if (node.g > 0)
            {
                RetracePath(node.parent);
            }
        }

    }
}