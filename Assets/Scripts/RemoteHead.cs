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
        if (RemoteHeadManager.Instance.activeHead != gameObject)
        {
            RemoteHeadManager.Instance.activeHead = gameObject;
            ToolManager.Instance.SetActiveTool(gameObject);
        }
        else
        {
            RemoteHeadManager.Instance.activeHead = null;
            ToolManager.Instance.SetActiveTool(null);
        }
    }
}
