using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class NoteManager : Singleton<NoteManager>
{
    public Note ActiveNote;
    public Note DefaultNote;

    public void SetActiveNote(Note noteObject)
    {
        ActiveNote.SetColor(new Color(0, 0, 255, 127));
        ActiveNote = noteObject;
        ActiveNote.SetColor(new Color(255, 0, 255, 127));
    }

    public void SetDefaultNote()
    {
        SetActiveNote(DefaultNote);
    }

    public void ClearCanvas() {
        ActiveNote.GetComponentInChildren<DrawCanvas>().ClearCanvas();
    }

}
