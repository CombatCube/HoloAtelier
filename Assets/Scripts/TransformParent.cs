using UnityEngine;
using HoloToolkit.Unity;

public class TransformParent : MonoBehaviour {

    private GameObject parent;
    private Vector3 parentStart;
    private Vector3 handStart;
	// Use this for initialization
	void Start () {
        parent = gameObject.transform.parent.gameObject;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void PerformManipulationStart(Vector3 position)
    {
        Billboard billboard = parent.AddComponent<Billboard>();
        Vector3 parentStart = parent.transform.localPosition;
        Vector3 handStart = position;   
    }

    void PerformManipulationUpdate(Vector3 position)
    {
        parent.transform.position = Vector3.Lerp(parent.transform.position, position, 0.2f);
    }

    void PerformManipulationCompleted()
    {
        Destroy(parent.GetComponent<Billboard>());
    }

    void PerformManipulationCanceled()
    {
        Destroy(parent.GetComponent<Billboard>());
    }

}
