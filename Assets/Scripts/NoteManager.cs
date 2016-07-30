﻿using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using System.Collections.Generic;
using System;

public class NoteManager : Singleton<NoteManager>
{
    public Note NoteObject2D;
    public Note NoteObject3D;
    public VoiceNote VoiceNoteObject;
    public Draw3D DrawTool;
    public Material CanvasMaterial;
    public Note ActiveNote;
    public Note DefaultNote;

    private class CanvasKey
    {
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
    void Start()
    {
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Draw3DStroke] = this.OnReceive3DStroke;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.VoiceNote] = this.OnReceiveVoiceNote;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnReceive3DStroke(NetworkInMessage msg)
    {
        Debug.Log("Received Draw3DStroke.");

        long userID = msg.ReadInt64();
        long noteID = msg.ReadInt64();
        byte noteType = msg.ReadByte();
        Vector3 localPosition = CustomMessages.Instance.ReadVector3(msg);
        Quaternion localRotation = CustomMessages.Instance.ReadQuaternion(msg);
        Vector3 localScale = CustomMessages.Instance.ReadVector3(msg);
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
            note = CreateNewNote(userID, noteType, localPosition, localRotation, localScale);
            notes.Add(new CanvasKey(userID, noteID), note);
        }
        note.GetComponentInChildren<DrawCanvas>().DrawLine(list.ToArray());
    }


    private void OnReceiveVoiceNote(NetworkInMessage msg)
    {
        Debug.Log("Received voice note.");

        long userID = msg.ReadInt64();
        long noteID = msg.ReadInt64();
        byte noteType = msg.ReadByte();
        Vector3 localPosition = CustomMessages.Instance.ReadVector3(msg);
        Quaternion localRotation = CustomMessages.Instance.ReadQuaternion(msg);
        Vector3 localScale = CustomMessages.Instance.ReadVector3(msg);
        string text = msg.ReadString();
        var data = new List<float>();
        while (msg.GetUnreadBitsCount() > 0)
        {
            data.Add(msg.ReadFloat());
        }
        Note note;
        CanvasKey key = new CanvasKey(userID, noteID);
        if (notes.ContainsKey(key))
        {
            note = notes[key];
        }
        else
        {
            note = CreateNewNote(userID, noteType, localPosition, localRotation, localScale);
            notes.Add(new CanvasKey(userID, noteID), note);
        }
        note.GetComponentInChildren<UnityEngine.UI.Text>().text = text;
        note.GetComponent<AudioClip>().SetData(data.ToArray(), 0);
    }

    public VoiceNote CreateVoiceNote(long userID, Vector3 localPosition, Quaternion localRotation)
    {
        VoiceNote newNoteObject = Instantiate(VoiceNoteObject);
        newNoteObject.transform.SetParent(this.gameObject.transform, false);
        newNoteObject.transform.localPosition = localPosition;
        newNoteObject.transform.localRotation = localRotation;
        newNoteObject.userID = userID;
        return newNoteObject;
    }

    public Note CreateNewNote(long userID, byte noteType, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
    {
        Note newNoteObject;
        if (noteType == (byte)Note.NoteType.Draw2D)
        {
            newNoteObject = Instantiate(NoteObject2D);
        }
        else // Draw3D
        {
            newNoteObject = Instantiate(NoteObject3D);
        }
        newNoteObject.DrawTool = DrawTool;
        newNoteObject.GetComponentInChildren<MeshRenderer>().material = CanvasMaterial;
        newNoteObject.GetComponentInChildren<MeshRenderer>().transform.localScale = localScale;
        newNoteObject.transform.SetParent(this.gameObject.transform, false);
        newNoteObject.transform.localPosition = localPosition;
        newNoteObject.transform.localRotation = localRotation;
        newNoteObject.transform.localScale = Vector3.one;
        newNoteObject.userID = userID;
        notes.Add(new CanvasKey(CustomMessages.Instance.localUserID, newNoteObject.GetInstanceID()), newNoteObject);
        return newNoteObject;
    }

    public void SetActiveNote(Note noteObject)
    {
        ActiveNote.SetColor(new Color(0, 0, 255, 127));
        ActiveNote = noteObject;
        ActiveNote.SetColor(new Color(0, 255, 0, 127));
    }

    public void SetDefaultNote()
    {
        SetActiveNote(DefaultNote);
    }

    public void ClearCanvas() {
        if (ActiveNote != DefaultNote)
        {
            Note note = ActiveNote;
            SetDefaultNote();
            notes.Remove(new CanvasKey(note.userID, note.GetInstanceID()));
            Destroy(note.gameObject);
        } else
        {
            ActiveNote.GetComponentInChildren<DrawCanvas>().ClearCanvas();
        }

    }

}
