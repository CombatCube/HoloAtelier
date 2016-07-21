using UnityEngine;
using GestureRecognizer;
using System.Collections;

public class DrawingRecognizer : MonoBehaviour {

    public GestureBehaviour GestureBehaviour;

	// Use this for initialization
	void Start ()
    {
        GestureBehaviour.OnGestureRecognition += OnRecognition;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnDestroy()
    {
        GestureBehaviour.OnGestureRecognition -= OnRecognition;
    }

    void OnRecognition(Gesture g, Result r)
    {
        Debug.Log("Gesture is " + r.Name + " and scored: " + r.Score);
        if (r.Score > 0.01f)
        {
            if (r.Name == "rectangle")
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                float length = g.GetOriginalPathLength();
                Debug.Log("Path length: " + length);
                cube.transform.localScale *= (length / 4f);
                cube.transform.position = GestureBehaviour.transform.position;
                cube.transform.rotation = GestureBehaviour.transform.localRotation;
                cube.transform.SetParent(gameObject.transform, true);
            }
            if (r.Name == "circle")
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                float length = g.GetOriginalPathLength();
                Debug.Log("Path length: " + length);
                sphere.transform.localScale *= (length / Mathf.PI);
                sphere.transform.position = GestureBehaviour.transform.position;
                sphere.transform.rotation = GestureBehaviour.transform.localRotation;
                sphere.transform.SetParent(gameObject.transform, true);
            }
        }
    }
}
