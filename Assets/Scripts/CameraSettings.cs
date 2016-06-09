using UnityEngine;
using System.Collections;

public class CameraSettings : MonoBehaviour {

	// Use this for initialization
	void Start () {
        // Allow user to appreciate holograms up close
        Camera.main.nearClipPlane = 0.0001f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
