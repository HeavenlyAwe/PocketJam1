using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LevelGenerator : MonoBehaviour {

    public GameObject box1Prefab;
    public GameObject box2Prefab;
    public GameObject box3Prefab;

    public GameObject orangePrefab;

    public float slopeThickness = 0.2f;

    // public TextAsset level1;
    public Texture2D level1;
    public Texture2D level2;

    private Texture2D level;

    public string levelPath;

    private Vector3 playerStartPosition;
    public Vector3 PlayerStartPosition {
        get { return playerStartPosition; }
    }

    private bool[][] checkedValues;

    // Use this for initialization
    void Start() {
        // parseTextData();

        level = level2;
        level = level1; // TODO: Remove this hard-coded value!
        parsePictureData();
    }

    private void parseTextData() {
        StreamReader inputStream = new StreamReader(levelPath);

        int iy = 0;
        while (!inputStream.EndOfStream) {
            string line = inputStream.ReadLine();

            // Loop over all the characters in the current line, to generate the level
            CharEnumerator characterEnumerator = line.GetEnumerator();
            int ix = 0;
            while (characterEnumerator.MoveNext()) {
                char c = characterEnumerator.Current;
                if (c == 'x') {
                    addTile(0, ix, iy);
                } else if (c == 'a') {
                    addTile(1, ix, iy);
                } else if (c == 'p') {
                    setPlayerStartPosition(ix, iy);
                } else if (c == 'R') {
                    // Handle right slope
                } else if (c == 'o') {
                    addOrange(ix, iy);
                }
                ix++;
            }
            iy++;
        }

        inputStream.Close();
    }

    private void parsePictureData() {
        Color32[] colors = level.GetPixels32();
        checkedValues = new bool[level.height][];
        for (int i = 0; i < level.height; i++) {
            checkedValues[i] = new bool[level.width];
            for (int j = 0; j < level.width; j++) {
                checkedValues[i][j] = false;
            }
        }

        for (int i = 0; i < colors.Length; i++) {
            int x = i % level.width;
            int y = i / level.width;

            if (checkedValues[y][x]) {
                continue;
            }

            int r = colors[i].r;
            int g = colors[i].g;
            int b = colors[i].b;

            if (r == 0 && g == 63 && b == 0) {

            } else if (r == 63 && g == 63 && b == 63) {
                // addTile(0, x, y);
                addSlope(0, 1, x, y, colors, r, g, b);
            } else if (r == 127 && g == 127 && b == 127) {
                // addTile(1, x, y);
                addSlope(0, -1, x, y, colors, r, g, b);
            } else if (r == 0 && g == 0 && b == 0) {
                addTile(2, x, y);
            } else if (r == 255 && g == 0 && b == 0) {
                setPlayerStartPosition(x, y);
            } else if (r == 0 && g == 255 && b == 0) {
                addOrange(x, y);
            }

            checkedValues[y][x] = true;
        }

        addTile(2, 0, 0);
    }

    /// <summary>
    /// Direction should be either 1 or -1.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="direction"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="colors"></param>
    /// <param name="r"></param>
    /// <param name="b"></param>
    /// <param name="g"></param>
    private void addSlope(int type, int direction, int x, int y, Color32[] colors, int r, int b, int g) {
        int maxX = x;
        int maxY = y;

        for (int ix = x; ix < level.width; ix++) {
            int i = y * level.width + ix;

            int rr = colors[i].r;
            int gg = colors[i].g;
            int bb = colors[i].b;

            if (rr != r || gg != g || bb != b) {
                break;
            }
            maxX = ix;
        }

        for (int iy = y; iy < level.height; iy++) {
            int i = iy * level.width + x;

            int rr = colors[i].r;
            int gg = colors[i].g;
            int bb = colors[i].b;

            if (rr != r || gg != g || bb != b) {
                break;
            }
            maxY = iy;
        }

        for (int iy = y; iy <= maxY; iy++) {
            for (int ix = x; ix <= maxX; ix++) {
                checkedValues[iy][ix] = true;
            }
        }

        float x0 = x - 0.5f;
        float y0 = y - 0.5f;
        float x1 = maxX + 0.5f;
        float y1 = maxY + 0.5f;

        float length = Mathf.Sqrt(Mathf.Pow(x1 - x0, 2) + Mathf.Pow(y1 - y0, 2));

        GameObject slopeCenter = GameObject.CreatePrimitive(PrimitiveType.Cube);

        float angle = Mathf.Acos((x1 - x0) / length) * direction;
        float xOffset = slopeThickness/2.0f * Mathf.Sin(angle);
        float yOffset = slopeThickness / 2.0f * Mathf.Cos(angle);

        slopeCenter.transform.localScale = new Vector3(length, slopeThickness, 1.0f);
        slopeCenter.transform.Rotate(new Vector3(0, 0, -Mathf.Rad2Deg * angle));
        slopeCenter.transform.position = new Vector3((x1 - x0) / 2.0f + x0 - xOffset, (y1 - y0) / 2.0f + y0 - yOffset, 0);

    }

    private void addTile(int type, int ix, int iy) {
        GameObject tile = null; // = GameObject.CreatePrimitive(PrimitiveType.Cube);

        if (type == 0) {
            tile = (GameObject)Instantiate(box1Prefab, new Vector3(ix, iy, 0), Quaternion.Euler(0, 0, 180));
            tile.name = "Tile: Box1";
        } else if (type == 1) {
            tile = (GameObject)Instantiate(box2Prefab, new Vector3(ix, iy, 0), Quaternion.Euler(0, 0, 180));
            tile.name = "Tile: Box2";
        } else if (type == 2) {
            tile = (GameObject)Instantiate(box3Prefab, new Vector3(ix, iy, 0), Quaternion.Euler(0, 0, 180));
            tile.name = "Tile: Box3";
        }
        tile.transform.SetParent(this.transform);
    }

    private void setPlayerStartPosition(int ix, int iy) {
        playerStartPosition = new Vector3(ix, iy, 0);
        GameObject.FindGameObjectWithTag("Player").transform.position = PlayerStartPosition;
    }

    private void addOrange(int ix, int iy) {
        GameObject orange = (GameObject)Instantiate(orangePrefab, new Vector3(ix, iy, 0), Quaternion.identity);
        orange.transform.SetParent(this.transform);
    }
}
