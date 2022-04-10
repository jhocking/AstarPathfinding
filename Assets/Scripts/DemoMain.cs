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
        var blankImage = new Texture2D(1, 1);
        blankImage.SetPixel(0, 0, Color.white);
        blankImage.Apply();
        var blankTile = tileObj.AddComponent<SpriteRenderer>();
        blankTile.sprite = Sprite.Create(blankImage, new Rect(0, 0, 1, 1), Vector2.zero, 1);

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
                        tile.color = new Color(.9f, .95f, .9f);
                        break;
                    case 2:
                        tile.color = new Color(.95f, .9f, .85f);
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
        var startDrag = startTile.gameObject.AddComponent<DraggableObject>();
        startDrag.OnEndDrag += PlaceTiles;
        startTile.transform.position = Vector3.zero;
        startTile.color = Color.green;

        endTile = Instantiate(blankTile);
        endTile.gameObject.name = "End";
        var endDrag = endTile.gameObject.AddComponent<DraggableObject>();
        endDrag.OnEndDrag += PlaceTiles;
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

    private void PlaceTiles()
    {
        Vector3 tmp;

        tmp = startTile.transform.position;
        startTile.transform.position = new Vector3((int)tmp.x, (int)tmp.y, 0);

        tmp = endTile.transform.position;
        endTile.transform.position = new Vector3((int)tmp.x, (int)tmp.y, 0);
    }

}
