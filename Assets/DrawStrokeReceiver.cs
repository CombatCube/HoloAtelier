using UnityEngine;
using System.Collections;
using HoloToolkit.Sharing;
using System.Collections.Generic;

public class DrawStrokeReceiver : MonoBehaviour {

    public DrawCanvas CanvasObject;
    public Material ReceivedCanvasMaterial;
    private class CanvasKey {
        long userID;
        long canvasID;

        public CanvasKey(long userID, long canvasID)
        {
            this.userID = userID;
            this.canvasID = canvasID;
        }
    }

    Dictionary<CanvasKey, DrawCanvas> canvases = new Dictionary<CanvasKey, DrawCanvas>();

	// Use this for initialization
	void Start () {
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Draw3DStroke] = this.OnReceive3DStroke;
    }

    // Update is called once per frame
    void Update () {
	
	}

    private void OnReceive3DStroke(NetworkInMessage msg)
    {
        Debug.Log("Received Draw3DStroke.");
        
        long userID = msg.ReadInt64();
        long canvasID = msg.ReadInt64();
        Vector3 localPosition = CustomMessages.Instance.ReadVector3(msg);
        Quaternion localRotation = CustomMessages.Instance.ReadQuaternion(msg);
        Vector3 localScale = CustomMessages.Instance.ReadVector3(msg);
        var list = new List<Vector3>();
        while (msg.GetUnreadBitsCount() > 0)
        {
            list.Add(CustomMessages.Instance.ReadVector3(msg));
        }

        DrawCanvas canvas;
        CanvasKey key = new CanvasKey(userID, canvasID);
        if (canvases.ContainsKey(key))
        {
            canvas = canvases[key];
        }
        else
        {
            canvas = CreateNewCanvas(userID, canvasID, localPosition, localRotation, localScale);
        }

        canvas.DrawLine(list.ToArray());
    }

    DrawCanvas CreateNewCanvas(long userID, long canvasID, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
    {
        DrawCanvas newCanvasObject = Instantiate(CanvasObject);
        newCanvasObject.transform.SetParent(this.gameObject.transform, false);
        newCanvasObject.transform.localPosition = localPosition;
        newCanvasObject.transform.localRotation = localRotation;
        newCanvasObject.transform.localScale = localScale;
        newCanvasObject.GetComponent<MeshRenderer>().material = ReceivedCanvasMaterial;
        canvases.Add(new CanvasKey(userID, canvasID), newCanvasObject);
        return newCanvasObject;
    }
}
