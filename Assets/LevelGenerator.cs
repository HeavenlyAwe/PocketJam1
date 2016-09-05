using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class LevelGenerator : MonoBehaviour {

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
                } else if (c == 'p') {
                    setPlayerStartPosition(ix, iy);
                }
                ix++;
            }
            iy++;
        }

        inputStream.Close();
    }

    private void addTile(int type, int ix, int iy) {
        GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.name = "Tile";
        tile.transform.SetParent(this.transform);
        tile.transform.position = new Vector3(ix, -iy, 0);
    }

    private void setPlayerStartPosition(int ix, int iy) {
        playerStartPosition = new Vector3(ix, -iy, 0);
        GameObject.FindGameObjectWithTag("Player").transform.position = PlayerStartPosition;
    }

}
