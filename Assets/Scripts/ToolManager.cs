using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System;

public partial class ToolManager : Singleton<ToolManager>
{
    public UnityEngine.UI.Text HudHelpText;
    public UnityEngine.UI.Image HudToolImage;

    public class ToolChangedEventArgs : EventArgs
    {
        public Tool newTool;
    }

    public event EventHandler<ToolChangedEventArgs> ToolChanged;

    public Tool ActiveTool { get; private set; }

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

    public void SetActiveTool(Tool tool)
    {
        if (ActiveTool != null)
        {
            ActiveTool.gameObject.SetActive(false);
        }
        if (tool != null)
        {
            tool.gameObject.SetActive(true);
        }
        ActiveTool = tool;
        EventHandler<ToolChangedEventArgs> toolChangedEvent = ToolChanged;
        ToolChangedEventArgs tcea = new ToolChangedEventArgs();
        tcea.newTool = tool;
        toolChangedEvent(this, tcea);
    }

    public void SetHelpText(string text)
    {
        if (text != null)
        {
            HudHelpText.text = text;
        }
        else
        {
            HudHelpText.text = "";
        }
    }

    public void SetHudImage(Sprite image)
    {
        if (image != null)
        {
            HudToolImage.sprite = image;
            HudToolImage.enabled = true;
        }
        else
        {
            HudToolImage.enabled = false;
            HudToolImage.sprite = null;
        }
    }
}
