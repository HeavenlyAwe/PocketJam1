using UnityEngine;
using System.Collections;

public class ParallaxScrolling : MonoBehaviour {
    
    public GameObject player;

    public float scrollSpeed;
    public float tileSizeZ;

    private Vector3 startPosition;

    void Start() {
        this.startPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
        this.transform.position = startPosition + new Vector3(1, 0, 0) * newPosition;
	}
}
