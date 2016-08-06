using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using System.Collections.Generic;

public class Draw3D : Tool {
    //private Vector3 manipulationPreviousPosition;
    private float penOffset = 0.1f;
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

    // Update is called once per frame
    void Update () {
        if (HandsManager.Instance.HandDetected && NoteManager.Instance.ActiveNote != null) {
            GetComponentInChildren<MeshRenderer>().enabled = true;
            Vector3 pos;
            HandsManager.Instance.Hand.properties.location.TryGetPosition(out pos);
            Note note = NoteManager.Instance.ActiveNote;
            if (note.DrawType == Note.NoteType.Draw3D)
            {
                ActiveCanvas = NoteManager.Instance.ActiveNote.GetComponentInChildren<DrawCanvas>();
                pos += penOffset * (Camera.main.transform.forward);
                gameObject.transform.position = pos;
            }
            else if (note.DrawType == Note.NoteType.Draw2D)
            {
                ActiveCanvas = NoteManager.Instance.ActiveNote.GetComponentInChildren<DrawCanvas>();
                Vector3 localPos = ActiveCanvas.transform.InverseTransformPoint(pos);
                Vector3 planePos = Vector3.ProjectOnPlane(localPos, Vector3.forward);
                gameObject.transform.position = ActiveCanvas.transform.TransformPoint(planePos);
            }
            else if (note.DrawType == Note.NoteType.Voice)
            {
                GetComponentInChildren<MeshRenderer>().enabled = false;
                ActiveCanvas = null;
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
        if (GestureManager.Instance.FocusedObject != null)
        {
            GestureManager.Instance.FocusedObject.SendMessage("OnSelect");
        }
    }

    void PerformManipulationStart(Vector3 position)
    {
        if (ActiveCanvas != null)
        {
            ActiveCanvas.StartLine(gameObject.transform.position);
        }
    }

    void PerformManipulationUpdate(Vector3 position)
    {
        if (GestureManager.Instance.manipulationTarget != null
            && ActiveCanvas != null)
        {
            ActiveCanvas.UpdateLine(gameObject.transform.position);
        }
    }

    void PerformManipulationCompleted()
    {
        if (ActiveCanvas != null)
        {
            // Send the stroke to the other HoloLens.
            Debug.Log("Sending Draw3DStroke.");
            ActiveCanvas.SendStroke();
        }
    }

    void PerformManipulationCanceled()
    {
        if (ActiveCanvas != null)
        {
            Debug.Log("Canceled Draw3DStroke.");
            ActiveCanvas.SendStroke();
        }
    }
}
