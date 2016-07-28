using UnityEngine;
using System.Collections;

public class RemotePerspective : Tool {

    private Tool lastTool;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public new void Activate()
    {
        lastTool = ToolManager.Instance.ActiveTool;
        ToolManager.Instance.SetActiveTool(lastTool);
    }

    void OnSelect ()
    {
        RemoteHeadManager.Instance.SetActiveHead(null);
        ToolManager.Instance.SetActiveTool(lastTool);
    }

}
