using UnityEngine;
using System.Collections;

public class RemoteHead : MonoBehaviour {

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {

	}

    void OnSelect()
    {
        Debug.Log("Remote Head tapped!");
        RemoteHeadManager.Instance.SetActiveHead(gameObject);
    }
}
