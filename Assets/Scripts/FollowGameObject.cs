using UnityEngine;
using System.Collections;

public class FollowGameObject : MonoBehaviour {

    public GameObject gameObjectToFollow;
    public Vector3 offset;
	
	// Update is called once per frame
	void Update () {
        transform.position = gameObjectToFollow.transform.position + offset;
    }
}
