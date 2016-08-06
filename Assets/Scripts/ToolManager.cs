using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System;

public partial class ToolManager : Singleton<ToolManager>
{
    public UnityEngine.UI.Text HudHelpText;
    public UnityEngine.UI.Image HudToolImage;
    public Tool DefaultTool;
    private float deltaTime = 0;

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
        deltaTime += Time.deltaTime;
        if (deltaTime > 5)
        {
            HudHelpText.enabled = false;
        }
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
            SetHUD(tool.HelpText, tool.HudImage);
        }
        else
        {
            SetHUD(null, null);
            DefaultTool.gameObject.SetActive(true);
        }
        ActiveTool = tool;
        EventHandler<ToolChangedEventArgs> toolChangedEvent = ToolChanged;
        ToolChangedEventArgs tcea = new ToolChangedEventArgs();
        tcea.newTool = tool;
        toolChangedEvent(this, tcea);
    }

    public void SetHUD(string text, Sprite image)
    {
        deltaTime = 0;
        if (text != null)
        {
            HudHelpText.text = text;
            HudHelpText.enabled = true;
        }
        else
        {
            HudHelpText.text = "";
            HudHelpText.enabled = false;
        }
        if (image != null)
        {
            HudToolImage.sprite = image;
            HudToolImage.enabled = true;
        }
        else
        {
            HudToolImage.sprite = null;
            HudToolImage.enabled = false;
        }
    }
}
