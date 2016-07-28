using UnityEngine;
using HoloToolkit.Unity;

public class RemoteHead : MonoBehaviour {

    private GazeStabilizer gazeStabilizer;

    void Awake()
    {
        gazeStabilizer = GetComponent<GazeStabilizer>();
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        transform.localPosition = gazeStabilizer.StableHeadPosition;
	}

    void OnSelect()
    {
        Debug.Log("Remote Head tapped!");
        //RemoteHeadManager.Instance.SetActiveHead(gameObject);
    }
}
