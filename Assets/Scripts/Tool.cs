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
        }
        else
        {
            ToolManager.Instance.SetActiveTool(null);
        }
    }
}
