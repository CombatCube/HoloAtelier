using UnityEngine;
using GestureRecognizer;
using System.Collections;

public class DrawingRecognizer : MonoBehaviour {

    public GestureBehaviour GestureBehaviour;
    public Material CubeMaterial;

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
            if (r.Name == "square")
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                float length = g.GetOriginalPathLength();
                Debug.Log("Path length: " + length);
                cube.transform.localScale *= (length / 4f);
                cube.transform.position = GestureBehaviour.transform.position + (Vector3)g.GetOriginalCenter();
                cube.transform.rotation = GestureBehaviour.transform.localRotation;
                cube.transform.SetParent(gameObject.transform, true);
                cube.GetComponent<MeshRenderer>().material = CubeMaterial;
                cube.AddComponent<Rigidbody>();
            }
            if (r.Name == "circle")
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                float length = g.GetOriginalPathLength();
                Debug.Log("Path length: " + length);
                sphere.transform.localScale *= (length / Mathf.PI);
                sphere.transform.position = GestureBehaviour.transform.position + (Vector3)g.GetOriginalCenter();
                sphere.transform.rotation = GestureBehaviour.transform.localRotation;
                sphere.transform.SetParent(gameObject.transform, true);
                sphere.GetComponent<MeshRenderer>().material = CubeMaterial;
                Rigidbody body = sphere.AddComponent<Rigidbody>();
                body.mass = 0.2f;
            }
        }
    }
}
