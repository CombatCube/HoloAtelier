using UnityEngine;
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
    public KeywordManager KeywordManager;
    public bool recording;

    Dictionary<string, Note> notes = new Dictionary<string, Note>();

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
        string noteID = msg.ReadString();
        byte noteType = msg.ReadByte();
        Vector3 localPosition = CustomMessages.Instance.ReadVector3(msg);
        Quaternion localRotation = CustomMessages.Instance.ReadQuaternion(msg);
        var list = new List<Vector3>();
        while (msg.GetUnreadBitsCount() > 0)
        {
            list.Add(CustomMessages.Instance.ReadVector3(msg));
        }

        Note note;
        string key = noteID;
        if (notes.ContainsKey(key))
        {
            note = notes[key];
        }
        else
        {
            note = CreateNewNote(noteID, noteType, localPosition, localRotation);
        }
        note.GetComponentInChildren<DrawCanvas>().DrawLine(list.ToArray());
    }


    private void OnReceiveVoiceNote(NetworkInMessage msg)
    {
        Debug.Log("Received voice note.");

        long userID = msg.ReadInt64();
        string noteID = msg.ReadString();
        byte noteType = msg.ReadByte();
        Vector3 localPosition = CustomMessages.Instance.ReadVector3(msg);
        Quaternion localRotation = CustomMessages.Instance.ReadQuaternion(msg);
        string text = msg.ReadString();
        var data = new List<float>();
        while (msg.GetUnreadBitsCount() > 0)
        {
            data.Add(msg.ReadFloat());
        }
        VoiceNote note = CreateVoiceNote(noteID, localPosition, localRotation);
        note.GetComponentInChildren<UnityEngine.UI.Text>().text = text;
        AudioClip clip = AudioClip.Create(noteID, data.Count, 1, 48000, false);
        clip.SetData(data.ToArray(), 0);
        note.dictationAudio.clip = clip;
    }

    public VoiceNote CreateVoiceNote(string noteID, Vector3 localPosition, Quaternion localRotation)
    {
        VoiceNote newNoteObject = Instantiate(VoiceNoteObject);
        newNoteObject.transform.SetParent(this.gameObject.transform, false);
        newNoteObject.transform.localPosition = localPosition;
        newNoteObject.transform.localRotation = localRotation;
        if (noteID == "")
        {
            newNoteObject.noteID = SystemInfo.deviceUniqueIdentifier + "-" + newNoteObject.GetInstanceID();
        }
        else
        {
            newNoteObject.noteID = noteID;
        }
        notes.Add(newNoteObject.noteID, newNoteObject);
        return newNoteObject;
    }

    public Note CreateNewNote(string noteID, byte noteType, Vector3 localPosition, Quaternion localRotation)
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
        newNoteObject.transform.SetParent(this.gameObject.transform, false);
        newNoteObject.transform.localPosition = localPosition;
        newNoteObject.transform.localRotation = localRotation;
        newNoteObject.transform.localScale = Vector3.one;
        if (noteID == "")
        {
            newNoteObject.noteID = SystemInfo.deviceUniqueIdentifier + "-" + newNoteObject.GetInstanceID();
        }
        else
        {
            newNoteObject.noteID = noteID;
        }
        notes.Add(newNoteObject.noteID, newNoteObject);
        return newNoteObject;
    }

    public void SetActiveNote(Note noteObject)
    {
        ActiveNote = noteObject;
    }

    public void ClearCanvas()
    {
        Note note = ActiveNote;
        SetActiveNote(null);
        Destroy(note);
    }
}
