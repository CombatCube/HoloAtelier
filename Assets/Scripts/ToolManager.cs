using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System;

public partial class ToolManager : Singleton<ToolManager>
{
    public class ToolChangedEventArgs : EventArgs
    {
        public GameObject newTool;
    }

    public event EventHandler<ToolChangedEventArgs> ToolChanged;

    public GameObject ActiveTool { get; private set; }

    void Awake ()
    {

    }

    // Use this for initialization
    void Start ()
    {
        SetActiveTool(null);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {

    }

    public void SetActiveTool(GameObject tool)
    {
        if (ActiveTool != null)
        {
            ActiveTool.SetActive(false);
        }
        if (tool != null)
        {
            tool.SetActive(true);
        }
        ActiveTool = tool;
        EventHandler<ToolChangedEventArgs> toolChangedEvent = ToolChanged;
        ToolChangedEventArgs tcea = new ToolChangedEventArgs();
        tcea.newTool = tool;
        toolChangedEvent(this, tcea);
    }

}
