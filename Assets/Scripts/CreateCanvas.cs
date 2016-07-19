using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using System.Collections.Generic;

public class CreateCanvas : MonoBehaviour {

    public DrawCanvas CanvasObject;
    public Draw3D DrawTool;
    public Transform Canvases;

    private DrawCanvas newCanvasObject;
	// Use this for initialization
	void Start () {
	
	}

    public void Activate()
    {
        if (ToolManager.Instance.ActiveTool != gameObject)
        {
            ToolManager.Instance.SetActiveTool(gameObject);
            newCanvasObject = Instantiate(CanvasObject);
            newCanvasObject.transform.SetParent(this.gameObject.transform, false);
            newCanvasObject.transform.localPosition = Vector3.zero;
            newCanvasObject.transform.localEulerAngles = new Vector3(0, 180, 0);
            newCanvasObject.transform.localScale = Vector3.one;
        }
        else
        {
            ToolManager.Instance.SetActiveTool(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (HandsManager.Instance.TwoHandsDetected)
        {
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.enabled = true;
            }
            Vector3 pos1, pos2;
            HandsManager.Instance.Hands[0].properties.location.TryGetPosition(out pos1);
            HandsManager.Instance.Hands[1].properties.location.TryGetPosition(out pos2);
            gameObject.transform.position = (pos1 + pos2) / 2;
            pos1 = gameObject.transform.InverseTransformPoint(pos1);
            pos2 = gameObject.transform.InverseTransformPoint(pos2);
            float scalex = Mathf.Abs(pos2.x - pos1.x);
            float scaley = Mathf.Abs(pos2.y - pos1.y);
            newCanvasObject.transform.localScale = new Vector3(scalex, scaley, 1);
            gameObject.transform.LookAt(Camera.main.transform);
        }
        else
        {
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.enabled = false;
            }
        }
    }

    void OnSelect()
    {
        DrawTool.DrawCanvas = newCanvasObject;
        newCanvasObject.gameObject.transform.SetParent(Canvases, true);
        newCanvasObject.DrawTool = DrawTool;
        DrawTool.Activate();
    }
}
