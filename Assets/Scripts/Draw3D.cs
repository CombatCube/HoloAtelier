using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using System.Collections.Generic;

public class Draw3D : MonoBehaviour {
    //private Vector3 manipulationPreviousPosition;
    private float penOffset = 0.1f;

    // Set DrawCanvas to be the GameObject whose origin will be the reference for strokes.
    public DrawCanvas DrawCanvas;


    // Use this for initialization
    void Start () {

        // And when a new user join we will send the anchor transform we have.
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;

    }

    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
        // TODO: Send already-drawn strokes
    }

    public void Activate()
    {
        Transform mesh = DrawCanvas.transform.FindChild("Quad");
        if (ToolManager.Instance.ActiveTool != gameObject)
        {
            if (mesh != null)
            {
                mesh.gameObject.SetActive(true);
            }
            ToolManager.Instance.SetActiveTool(gameObject);
        }
        else
        {
            if (mesh != null)
            {
                mesh.gameObject.SetActive(false);
            }
            ToolManager.Instance.SetActiveTool(null);
        }
    }

    // Update is called once per frame
    void Update () {
        if (HandsManager.Instance.HandDetected) {
            GetComponentInChildren<MeshRenderer>().enabled = true;
            Vector3 pos;
            HandsManager.Instance.Hand.properties.location.TryGetPosition(out pos);
            if (DrawCanvas.DrawType == DrawCanvas.DrawMode.Draw3D)
            {
                pos += penOffset * (Camera.main.transform.forward);
                gameObject.transform.position = pos;
            }
            else if (DrawCanvas.DrawType == DrawCanvas.DrawMode.Draw2D)
            {
                // Get hand position relative to canvas origin.
                Vector3 localPos = DrawCanvas.transform.InverseTransformPoint(pos);
                // Project down onto canvas plane.
                Vector3 planePos = Vector3.ProjectOnPlane(localPos, Vector3.forward);
                // Set world pos of tool to drawing location.
                gameObject.transform.position = DrawCanvas.transform.TransformPoint(planePos);
            }

            Quaternion v = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
            gameObject.transform.rotation = v;
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().enabled = true;
        }
    }

    void OnSelect()
    {
        // Pass through to gazed object
        GestureManager.Instance.FocusedObject.SendMessage("OnSelect");
    }

    void PerformManipulationStart(Vector3 position)
    {
        DrawCanvas.StartLine(gameObject.transform.position);
    }

    void PerformManipulationUpdate(Vector3 position)
    {
        if (GestureManager.Instance.manipulationTarget != null)
        {
            DrawCanvas.UpdateLine(gameObject.transform.position);
        }
    }

    void PerformManipulationCompleted()
    {
        // Send the stroke to the other HoloLens.
        Debug.Log("Sending Draw3DStroke.");
        DrawCanvas.SendStroke();
    }

    void PerformManipulationCanceled()
    {
        Debug.Log("Canceled Draw3DStroke.");
        DrawCanvas.SendStroke();
    }

}
