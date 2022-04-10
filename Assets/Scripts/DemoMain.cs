using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astar;

public class DemoMain : MonoBehaviour
{
    private int[] walkableValues;
    private int[,] levelData;

    void Start()
    {
        walkableValues = new int[] {1, 2};

        levelData = new int[,] {
            {1, 1, 1, 1, 5, 1, 2, 2},
            {6, 6, 6, 1, 5, 1, 5, 2},
            {2, 2, 1, 1, 5, 1, 5, 2},
            {2, 2, 1, 1, 5, 1, 5, 2},
            {2, 2, 6, 6, 5, 1, 5, 2},
            {2, 2, 1, 1, 5, 1, 5, 2},
            {2, 2, 1, 1, 1, 1, 5, 2},
        };
    }
}
