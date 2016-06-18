using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using System.Collections.Generic;

public class Draw3D : MonoBehaviour {
    //private Vector3 manipulationPreviousPosition;
    private GameObject lastLineObject;
    private LineRenderer lastLine;
    private Vector3[] lastPoints;
    public Material DrawMaterial;
    public float DrawThreshold;

    // Use this for initialization
    void Start () {
        // We care about getting updates for the anchor transform.
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Draw3DStroke] = this.OnReceive3DStroke;

        // And when a new user join we will send the anchor transform we have.
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;

    }

    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
        // TODO: Send already-drawn strokes
    }

    private void OnReceive3DStroke(NetworkInMessage msg)
    {
        Debug.Log("Received Draw3DStroke.");
        
        // Eat the ID section
        long userID = msg.ReadInt64();

        GameObject lineObject = new GameObject();
        lineObject.transform.SetParent(gameObject.transform);
        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.material = DrawMaterial;
        line.useWorldSpace = true;
        line.SetWidth(DrawThreshold, DrawThreshold);
        var list = new List<Vector3>();
        while (msg.GetUnreadBitsCount() > 0)
        {
            list.Add(CustomMessages.Instance.ReadVector3(msg));
        }
        var lineArr = list.ToArray();
        line.SetVertexCount(lineArr.Length);
        line.SetPositions(lineArr);
    }

    // Update is called once per frame
    void Update () {
	
	}

    void PerformManipulationStart(Vector3 position)
    {
        lastLineObject = new GameObject();
        lastLineObject.transform.SetParent(gameObject.transform);
        lastLine = lastLineObject.AddComponent<LineRenderer>();
        lastLine.material = DrawMaterial;
        lastLine.useWorldSpace = true;
        lastLine.SetWidth(DrawThreshold, DrawThreshold);
        lastLine.SetVertexCount(1);

        Vector3[] points = new Vector3[1];
        points[0] = position;
        lastLine.SetPositions(points);
        lastPoints = points;
    }

    void PerformManipulationUpdate(Vector3 position)
    {
        if (GestureManager.Instance.IsManipulating)
        {
            int nextPointIdx = lastPoints.Length;
            if ((lastPoints[nextPointIdx - 1] - position).magnitude > DrawThreshold) {
                Vector3[] points = new Vector3[lastPoints.Length + 1];
                lastPoints.CopyTo(points, 0);
                points[nextPointIdx] = position;
                lastLine.SetVertexCount(lastPoints.Length + 1);
                lastLine.SetPositions(points);
                lastPoints = points;
            }
        }
    }

    void PerformManipulationCompleted()
    {
        // Send the stroke to the other HoloLens.
        Debug.Log("Sending Draw3DStroke.");
        CustomMessages.Instance.SendDraw3DStroke(lastPoints);
    }

    void OnUndo()
    {
        if(GestureManager.Instance.IsManipulating)
        {
            return;
        }
        GameObject.Destroy(gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject);
    }
}
