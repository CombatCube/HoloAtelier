using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using System.Collections.Generic;

public class Draw3D : MonoBehaviour {
    //private Vector3 manipulationPreviousPosition;
    private float penOffset = 0.1f;
    public Transform Tablet;
    DrawCanvas ActiveCanvas;

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
        if (ToolManager.Instance.ActiveTool != gameObject)
        {
            ToolManager.Instance.SetActiveTool(gameObject);
        }
        else
        {
            ToolManager.Instance.SetActiveTool(null);
        }
    }

    // Update is called once per frame
    void Update () {
        ActiveCanvas = NoteManager.Instance.ActiveNote.GetComponentInChildren<DrawCanvas>();
        if (HandsManager.Instance.HandDetected) {
            GetComponentInChildren<MeshRenderer>().enabled = true;
            Vector3 pos;
            HandsManager.Instance.Hand.properties.location.TryGetPosition(out pos);
            if (ActiveCanvas.DrawType == DrawCanvas.DrawMode.Draw3D)
            {
                pos += penOffset * (Camera.main.transform.forward);
                gameObject.transform.position = pos;
            }
            else if (ActiveCanvas.DrawType == DrawCanvas.DrawMode.Draw2D)
            {
                if (Tablet != null)
                {
                    // Get hand position relative to TABLET origin.
                    Vector3 localPos = Tablet.transform.InverseTransformPoint(pos);
                    // Project down onto TABLET plane.
                    Vector3 planePos = Vector3.ProjectOnPlane(localPos, Vector3.forward);
                    // Set world pos of tool to drawing location.
                    gameObject.transform.position = ActiveCanvas.transform.TransformPoint(planePos);
                }
                else
                {
                    // Get hand position relative to canvas origin.
                    Vector3 localPos = ActiveCanvas.transform.InverseTransformPoint(pos);
                    // Project down onto canvas plane.
                    Vector3 planePos = Vector3.ProjectOnPlane(localPos, Vector3.forward);
                    // Set world pos of tool to drawing location.
                    gameObject.transform.position = ActiveCanvas.transform.TransformPoint(planePos);
                }
            }

            Quaternion v = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
            gameObject.transform.rotation = v;
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }

    void OnSelect()
    {
        // Pass through to gazed object
        GestureManager.Instance.FocusedObject.SendMessage("OnSelect");
    }

    void PerformManipulationStart(Vector3 position)
    {
        ActiveCanvas.StartLine(gameObject.transform.position);
    }

    void PerformManipulationUpdate(Vector3 position)
    {
        if (GestureManager.Instance.manipulationTarget != null)
        {
            ActiveCanvas.UpdateLine(gameObject.transform.position);
        }
    }

    void PerformManipulationCompleted()
    {
        // Send the stroke to the other HoloLens.
        Debug.Log("Sending Draw3DStroke.");
        ActiveCanvas.SendStroke();
    }

    void PerformManipulationCanceled()
    {
        Debug.Log("Canceled Draw3DStroke.");
        ActiveCanvas.SendStroke();
    }

}
