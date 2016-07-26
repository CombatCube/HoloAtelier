using UnityEngine;
using System.Collections;
using HoloToolkit.Sharing;
using System.Collections.Generic;

public class DrawStrokeReceiver : MonoBehaviour {

    public Note NoteObject2D;
    public Note NoteObject3D;
    public Draw3D DrawTool;
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

    Dictionary<CanvasKey, Note> notes = new Dictionary<CanvasKey, Note>();

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
        long noteID = msg.ReadInt64();
        byte canvasType = msg.ReadByte();
        Vector3 localPosition = CustomMessages.Instance.ReadVector3(msg);
        Quaternion localRotation = CustomMessages.Instance.ReadQuaternion(msg);
        Vector3 quadScale = CustomMessages.Instance.ReadVector3(msg);
        var list = new List<Vector3>();
        while (msg.GetUnreadBitsCount() > 0)
        {
            list.Add(CustomMessages.Instance.ReadVector3(msg));
        }

        Note note;
        CanvasKey key = new CanvasKey(userID, noteID);
        if (notes.ContainsKey(key))
        {
            note = notes[key];
        }
        else
        {
            note = CreateNewNote(userID, noteID, canvasType, localPosition, localRotation, quadScale);
        }
        note.GetComponentInChildren<DrawCanvas>().DrawLine(list.ToArray());
    }

    Note CreateNewNote(long userID, long canvasID, byte canvasType, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
    {
        Note newNoteObject;
        if (canvasType == (byte)DrawCanvas.DrawMode.Draw2D)
        {
            newNoteObject = Instantiate(NoteObject2D);
        }
        else // Draw3D
        {
            newNoteObject = Instantiate(NoteObject3D);
        }
        newNoteObject.transform.SetParent(this.gameObject.transform, false);
        newNoteObject.transform.localPosition = localPosition;
        newNoteObject.transform.localRotation = localRotation;
        newNoteObject.DrawTool = DrawTool;
        newNoteObject.GetComponentInChildren<MeshRenderer>().material = ReceivedCanvasMaterial;
        newNoteObject.received = true;
        MeshRenderer node = newNoteObject.GetComponentInChildren<MeshRenderer>();
        node.transform.localScale = localScale;
        notes.Add(new CanvasKey(userID, canvasID), newNoteObject);
        return newNoteObject;
    }
}
