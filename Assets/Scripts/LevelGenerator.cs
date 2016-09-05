using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LevelGenerator : MonoBehaviour {

    public GameObject box1Prefab;
    public GameObject box2Prefab;
    public GameObject box3Prefab;

    public GameObject orangePrefab;

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
    private LineRenderer lineRenderer;

    // Use this for initialization
    void Start() {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        // parseTextData();

        level = level2;
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

            if (r == 63 && g == 63 && b == 63) {
                addTile(0, x, y);
                addSlope(0, x, y, colors, r, g, b);
            } else if (r == 127 && g == 127 && b == 127) {
                addTile(1, x, y);
            } else if (r == 0 && g == 0 && b == 0) {
                addTile(2, x, y);
            } else if (r == 255 && g == 0 && b == 0) {
                setPlayerStartPosition(x, y);
            } else if (r == 0 && g == 255 && b == 0) {
                addOrange(x, y);
            }
            
            checkedValues[y][x] = true;
        }
    }

    private void addSlope(int type, int x, int y, Color32[] colors, int r, int b, int g) {
        int maxX = x;
        int maxY = y;

        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));

        lineRenderer.SetColors(Color.blue, Color.blue);
        lineRenderer.SetWidth(0.2f, 0.2f);
        lineRenderer.SetVertexCount(2);

        for (int ix = x; ix < level.width; ix++) {
            int i = y * level.width + ix;

            int rr = colors[i].r;
            int gg = colors[i].g;
            int bb = colors[i].b;

            if (rr != r || gg != g || bb != b) {
                maxX = ix;
                break;
            }
        }

        for (int iy = y; iy < level.height; iy++) {
            int i = y * level.width + x;

            int rr = colors[i].r;
            int gg = colors[i].g;
            int bb = colors[i].b;

            if (rr != r || gg != g || bb != b) {
                maxY = iy;
                break;
            }
        }

        for (int iy = y; iy < maxY; iy++) {
            for (int ix = x; ix < maxX; ix++) {
                checkedValues[iy][ix] = true;
            }
        }

        lineRenderer.SetPosition(0, new Vector3(x, y, 0));
        lineRenderer.SetPosition(1, new Vector3(maxX, maxY, 0));
        Debug.Log(x + ":" + y + " - " + maxX + ":" + maxY);


        Mesh mesh = new Mesh();
        mesh.name = "testMesh";
        mesh.Clear();
        Vector3[] vertices = new Vector3[4];
        
        vertices[0] = new Vector3(-10.0f, 0.0f, 0.0f);
        vertices[1] = new Vector3(10.0f, 0.0f, 0.0f);
        vertices[2] = new Vector3(-10.0f, 50.0f, 0.0f);
        vertices[3] = new Vector3(10.0f, 50.0f, 0.0f);
        mesh.vertices = vertices;

        mesh.uv = new Vector2[4];
        mesh.uv[0] = new Vector2(0, 0);
        mesh.uv[1] = new Vector2(1, 0);
        mesh.uv[2] = new Vector2(0, 1);
        mesh.uv[3] = new Vector2(1, 1);

        mesh.triangles = new int[6];
        mesh.triangles[0] = 0;
        mesh.triangles[1] = 1;
        mesh.triangles[2] = 2;
        mesh.triangles[3] = 1;
        mesh.triangles[4] = 3;
        mesh.triangles[5] = 2;

        mesh.RecalculateNormals();

        GameObject theObj = new GameObject();
        theObj.AddComponent<MeshFilter>();
        theObj.AddComponent<MeshRenderer>();

        MeshFilter mf = (MeshFilter) theObj.gameObject.GetComponent(typeof(MeshFilter));
        MeshRenderer mr = (MeshRenderer) theObj.gameObject.GetComponent(typeof(MeshRenderer));
        mf.mesh = mesh;
        mr.material.color = Color.white;
        theObj.transform.SetParent(this.transform);
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
