using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LevelGenerator : MonoBehaviour {

    public GameObject box1Prefab;
    public GameObject box2Prefab;
    public GameObject box3Prefab;

    public GameObject orangePrefab;

    public string levelPath;

    private Vector3 playerStartPosition;
    public Vector3 PlayerStartPosition {
        get { return playerStartPosition; }
    }

    // Use this for initialization
    void Start() {
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

    private void addTile(int type, int ix, int iy) {
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Renderer rend = tile.GetComponent<Renderer>();
        rend.material.mainTexture = Resources.Load("Textures/cratetile256") as Texture;

        if (type == 0) {
            tile = (GameObject)Instantiate(box1Prefab, new Vector3(ix, -iy, 0), Quaternion.Euler(0, 0, 180));
            tile.name = "Tile: Box1";
        } else if (type == 1) {
            tile = (GameObject)Instantiate(box2Prefab, new Vector3(ix, -iy, 0), Quaternion.Euler(0, 0, 180));
            tile.name = "Tile: Box2";
        } else if (type == 2) {
            tile = (GameObject)Instantiate(box3Prefab, new Vector3(ix, -iy, 0), Quaternion.Euler(0, 0, 180));
            tile.name = "Tile: Box3";
        }
        tile.transform.SetParent(this.transform);
        tile.transform.position = new Vector3(ix, -iy, 0);
    }

    private void setPlayerStartPosition(int ix, int iy) {
        playerStartPosition = new Vector3(ix, -iy, 0);
        GameObject.FindGameObjectWithTag("Player").transform.position = PlayerStartPosition;
    }

    private void addOrange(int ix, int iy) {
        GameObject orange = (GameObject)Instantiate(orangePrefab, new Vector3(ix, -iy, 0), Quaternion.identity);
        orange.transform.SetParent(this.transform);
    }
}
