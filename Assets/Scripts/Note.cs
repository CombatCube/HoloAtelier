using UnityEngine;
using System.Collections;

public class Note : MonoBehaviour {

    public Draw3D DrawTool;

    public enum NoteType : byte
    {
        Draw3D,
        Draw2D,
        Voice
    }
    public NoteType DrawType;

    protected bool collapsed;

    private const float timeToCollapse = 0.2f;
    private float timePressed = timeToCollapse;

    public long userID;

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
            handle.material.color = new Color(255, 0, 0, 127);
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
        if (NoteManager.Instance.ActiveNote == this
            && NoteManager.Instance.DefaultNote != this)
        {
            timePressed = 0f;
            collapsed = true;
            NoteManager.Instance.SetDefaultNote();
            SetColor(new Color(255, 0, 0, 127));
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
            GetInstanceID(),
            (byte)DrawType,
            transform.localPosition,
            transform.localRotation,
            GetComponentInChildren<MeshRenderer>().transform.localScale,
            points
        );
    }
}
