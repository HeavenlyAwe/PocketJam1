using UnityEngine;
using System.Collections;

public class Restart : MonoBehaviour {
    
    public Vector3 playerStartPosition = new Vector3(0, 2, 0);

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Debug.Log("Player detected");
            LevelGenerator levelGenerator = GameObject.Find("Level").GetComponent<LevelGenerator>();
            Debug.Log(levelGenerator.PlayerStartPosition);
            other.transform.position = levelGenerator.PlayerStartPosition;
        }
    }
}
