using UnityEngine;

public class Tool : MonoBehaviour
{
    public string HelpText;
    public Sprite HudImage;

    public void Activate()
    {
        if (ToolManager.Instance.ActiveTool != this)
        {
            ToolManager.Instance.SetActiveTool(this);
            ToolManager.Instance.SetHelpText(HelpText);
            ToolManager.Instance.SetHudImage(HudImage);
        }
        else
        {
            ToolManager.Instance.SetActiveTool(null);
            ToolManager.Instance.SetHelpText(null);
            ToolManager.Instance.SetHudImage(null);
        }
    }
}
