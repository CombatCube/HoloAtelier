using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class FollowCamera : MonoBehaviour {

    public Vector3 LocalPosition;
    public Interpolator interp;
	// Use this for initialization
	void Start () {
        interp.PositionPerSecond = 3.0f;
	}
	
	// Update is called once per frame
	void Update () {
        interp.SetTargetPosition(Camera.main.transform.TransformPoint(LocalPosition));
	}
}
