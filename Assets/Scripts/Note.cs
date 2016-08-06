using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {
    
    public enum NoteType : byte
    {
        Draw3D,
        Draw2D,
        Voice
    }
    public NoteType DrawType;

    public bool collapsed;

    private const float timeToCollapse = 0.2f;
    private float timePressed = timeToCollapse;

    public string noteID;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
        timePressed += Time.deltaTime;
        Transform scaleTarget;
        MeshRenderer handle = GetComponentInChildren<MeshRenderer>();
        if (DrawType != NoteType.Voice)
        {
            scaleTarget = GetComponentInChildren<DrawCanvas>().transform;
        }
        else
        {
            scaleTarget = GetComponentInChildren<RectTransform>();
        }
        if (collapsed)
        {
            scaleTarget.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, timePressed / timeToCollapse);
        }
        else
        {
            scaleTarget.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, timePressed / timeToCollapse);
        }
    }

    // Select note to set active. If note is already active, collapse.
    public void OnSelect()
    {
        if (NoteManager.Instance.ActiveNote == this)
        {
            timePressed = 0f;
            collapsed = true;
            NoteManager.Instance.SetActiveNote(null);
        }
        else
        {
            if (collapsed)
            {
                timePressed = 0f;
                collapsed = false;
            }
            NoteManager.Instance.SetActiveNote(this);
        }
    }

    public void SetColor(Color color)
    {
        GetComponentInChildren<MeshRenderer>().material.color = color;
    }


    public void SendStroke(Vector3[] points)
    {
        DrawCanvas canvas = GetComponentInChildren<DrawCanvas>();
        CustomMessages.Instance.SendDraw3DStroke(
            noteID,
            (byte)DrawType,
            transform.localPosition,
            transform.localRotation,
            points
        );
    }
}
