using UnityEngine;
using System.Collections;
using HoloToolkit.Sharing;
using System.Collections.Generic;
using System;

public class DrawStrokeReceiver : MonoBehaviour {

    public DrawCanvas CanvasObject;

    Dictionary<Tuple<long, long>, DrawCanvas> canvases;

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
        var list = new List<Vector3>();
        while (msg.GetUnreadBitsCount() > 0)
        {
            list.Add(CustomMessages.Instance.ReadVector3(msg));
        }

        DrawCanvas canvas;
        Tuple<long, long> key = new Tuple<long, long>(userID, canvasID);
        if (canvases.ContainsKey(key))
        {
            canvas = canvases[key];
        }
        else
        {
            canvas = CreateNewCanvas(userID, canvasID, localPosition, localRotation);
        }

        canvas.DrawLine(list.ToArray());
    }

    DrawCanvas CreateNewCanvas(long userID, long canvasID, Vector3 localPosition, Quaternion localRotation)
    {
        DrawCanvas newCanvasObject = Instantiate(CanvasObject);
        newCanvasObject.transform.SetParent(this.gameObject.transform, false);
        newCanvasObject.transform.localPosition = localPosition;
        newCanvasObject.transform.localRotation = localRotation;
        newCanvasObject.transform.localScale = Vector3.one;
        canvases.Add(new Tuple<long, long>(userID, canvasID), newCanvasObject);
        return newCanvasObject;
    }
}
