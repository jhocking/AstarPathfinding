using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Astar;

public class DemoMain : MonoBehaviour
{
    private int[] walkableValues;
    private int[,] levelData;

    private SpriteRenderer startTile;
    private SpriteRenderer endTile;

    private SpriteRenderer pathDot;
    private GameObject pathRoot;

    void Start() {
        walkableValues = new int[] { 1, 2 };

        levelData = new int[,] {
            {1, 1, 1, 1, 5, 1, 2, 2},
            {6, 6, 6, 1, 5, 1, 5, 2},
            {2, 2, 1, 1, 5, 1, 5, 2},
            {2, 2, 1, 1, 5, 1, 5, 2},
            {2, 2, 6, 6, 5, 1, 5, 2},
            {2, 2, 1, 1, 5, 1, 5, 2},
            {2, 2, 1, 1, 1, 1, 5, 2},
        };

        // create template object that will be instantiated repeatedly
        var tileObj = new GameObject("Tile");
        var blankTile = tileObj.AddComponent<SpriteRenderer>();
        blankTile.sprite = Sprite.Create(new Texture2D(1, 1), new Rect(0, 0, 1, 1), Vector2.zero, 1);

        // create tile map
        var mapRoot = new GameObject("Map");
        mapRoot.transform.SetParent(this.transform);

        var width = levelData.GetLength(0);
        var height = levelData.GetLength(1);
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                var tile = Instantiate(blankTile, mapRoot.transform);
                tile.transform.position = new Vector3(i, j, 1); // background is further away
                switch (levelData[i, j])
                {
                    case 1:
                        tile.color = new Color(.9f, 1, .9f);
                        break;
                    case 2:
                        tile.color = new Color(1, .9f, .9f);
                        break;
                    case 5:
                        tile.color = new Color(.2f, .2f, 0);
                        break;
                    case 6:
                        tile.color = new Color(0, .2f, .2f);
                        break;
                }
            }
        }

        startTile = Instantiate(blankTile);
        startTile.gameObject.name = "Start";
        startTile.transform.position = Vector3.zero;
        startTile.color = Color.green;

        endTile = Instantiate(blankTile);
        endTile.gameObject.name = "End";
        endTile.transform.position = new Vector3(width - 1, height - 1, 0);
        endTile.color = Color.red;

        pathDot = Instantiate(blankTile);
        pathDot.gameObject.name = "Dot";
        pathDot.transform.position = new Vector3(0, 0, -100); // behind the camera
        pathDot.transform.localScale = new Vector3(.25f, .25f, .25f);
        pathDot.color = Color.grey;

        var pathRoot = new GameObject("Path");
        pathRoot.transform.SetParent(this.transform);

        // remove the template object once no longer needed
        Destroy(tileObj);
    }

}
