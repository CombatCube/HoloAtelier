using UnityEngine;
using System.Collections;

public class RemotePerspective : MonoBehaviour {

    private GameObject lastTool;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Activate ()
    {
        lastTool = ToolManager.Instance.ActiveTool;
        ToolManager.Instance.SetActiveTool(gameObject);
    }

    void OnSelect ()
    {
        RemoteHeadManager.Instance.SetActiveHead(null);
        ToolManager.Instance.SetActiveTool(lastTool);
    }

}
