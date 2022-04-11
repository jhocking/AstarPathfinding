using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Astar;

public class DemoMain : MonoBehaviour
{
    private int[] walkableValues;
    private int[,] levelData;

    private bool only4way;
    private Vector2 dragOffset;

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
        blankTile.sprite = Sprite.Create(blankImage, new Rect(0, 0, 1, 1), Vector2.zero, 1.05f);

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

        only4way = false;
        dragOffset = new Vector2(-.5f, -.5f);

        startTile = Instantiate(blankTile);
        startTile.gameObject.name = "Start";
        startTile.transform.position = Vector3.zero;
        startTile.color = Color.green;

        var startBox = startTile.gameObject.AddComponent<BoxCollider>();
        startBox.size = Vector3.one;
        var startDrag = startTile.gameObject.AddComponent<DraggableObject>();
        startDrag.OnStartDrag += DisposeOldPath;
        startDrag.OnEndDrag += PlaceTiles;
        startDrag.offset = dragOffset;

        endTile = Instantiate(blankTile);
        endTile.gameObject.name = "End";
        endTile.transform.position = new Vector3(width - 1, height - 1, 0);
        endTile.color = Color.red;

        var endBox = endTile.gameObject.AddComponent<BoxCollider>();
        endBox.size = Vector3.one;
        var endDrag = endTile.gameObject.AddComponent<DraggableObject>();
        endDrag.OnStartDrag += DisposeOldPath;
        endDrag.OnEndDrag += PlaceTiles;
        endDrag.offset = dragOffset;

        pathDot = Instantiate(blankTile);
        pathDot.gameObject.name = "Dot";
        pathDot.transform.position = new Vector3(0, 0, -100); // behind the camera
        pathDot.transform.localScale = new Vector3(.25f, .25f, .25f);
        pathDot.color = Color.grey;

        pathRoot = new GameObject("Path");
        pathRoot.transform.SetParent(this.transform);
        DoPathfinding();

        // remove the template object once no longer needed
        Destroy(tileObj);
    }

    // toggle between 4 and 8 way movement
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            only4way = !only4way;
            DisposeOldPath();
            DoPathfinding();
        }
    }
	void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 120, 20), only4way ? "4 way movement" : "8 way movement");
    }

	private void PlaceTiles()
    {
        Vector3 tmp;

        tmp = startTile.transform.position;
        startTile.transform.position = new Vector3((int)(tmp.x - dragOffset.x), (int)(tmp.y - dragOffset.y), 0);

        tmp = endTile.transform.position;
        endTile.transform.position = new Vector3((int)(tmp.x - dragOffset.x), (int)(tmp.y - dragOffset.y), 0);

        DoPathfinding();
    }

    private void DoPathfinding() {
#if UNITY_WEBGL
        var start = startTile.transform.position;
        var end = endTile.transform.position;
        var finder = new PathFinder((int)start.x, (int)start.y, (int)end.x, (int)end.y, only4way, levelData, walkableValues);
        ConstructNewPath(finder.Path);
#else
        StartCoroutine(ThreadedDoPathfinding());
#endif
    }

    // calculate map in separate thread, then generate mesh in unity thread
    // threaded process in unity https://stackoverflow.com/a/32234325/686008
    private IEnumerator ThreadedDoPathfinding()
    {
        var done = false;

        List<Vector2Int> path = null;
        var start = startTile.transform.position;
        var end = endTile.transform.position;

        new Thread(() => {
            var finder = new PathFinder((int)start.x, (int)start.y, (int)end.x, (int)end.y, only4way, levelData, walkableValues);
            path = finder.Path;

            done = true;
        }).Start();

        while (!done)
        {
            yield return null;
        }

        ConstructNewPath(path);
    }

    private void DisposeOldPath()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Respawn");
        foreach (GameObject go in objects)
        {
            Destroy(go);
        }
    }

    private void ConstructNewPath(List<Vector2Int> path) {
        foreach (Vector2Int step in path) {
            var dot = Instantiate(pathDot, pathRoot.transform);
            dot.gameObject.tag = "Respawn";
            dot.transform.position = new Vector3(step.x + .35f, step.y + .35f, -1);
        }
    }

}
