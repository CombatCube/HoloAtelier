using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {

    public Vector3 LocalPosition;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = Camera.main.transform.TransformPoint(LocalPosition);
	}
}
