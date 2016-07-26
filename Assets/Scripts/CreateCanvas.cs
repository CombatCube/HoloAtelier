using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using System.Collections.Generic;

public class CreateCanvas : MonoBehaviour {

    public Note NoteObject2D;
    public Note NoteObject3D;
    public Draw3D DrawTool;
    public Transform Notes;

    private Note newNoteObject;
    private DrawCanvas newCanvas;
    private MeshRenderer newQuad;

    private Vector3 startPosition;
	// Use this for initialization
	void Start () {
	
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
    void Update()
    {
        if (HandsManager.Instance.HandDetected)
        {
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.enabled = true;
            }
            if (GestureManager.Instance.manipulationTarget == null)
            {
                Vector3 pos;
                HandsManager.Instance.Hand.properties.location.TryGetPosition(out pos);
                gameObject.transform.position = pos;
            }
            // Lock gesture while
        }
        else
        {
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.enabled = false;
            }
        }
    }

    void OnSelect()
    {
        newNoteObject = Instantiate(NoteObject3D);
        newCanvas = newNoteObject.GetComponentInChildren<DrawCanvas>();
        newNoteObject.transform.position = transform.position;
        newNoteObject.transform.SetParent(Notes, true);
        NoteManager.Instance.SetActiveNote(newNoteObject);
        DrawTool.Activate();
    }

    void PerformManipulationStart(Vector3 position)
    {
        startPosition = position;
        newNoteObject = Instantiate(NoteObject2D);
        newCanvas = newNoteObject.GetComponentInChildren<DrawCanvas>();
        newQuad = newNoteObject.GetComponentInChildren<MeshRenderer>();
        newNoteObject.transform.SetParent(this.gameObject.transform, false);
        newNoteObject.transform.localPosition = Vector3.zero;
        newNoteObject.transform.localEulerAngles = new Vector3(0, 180, 0);
        newQuad.transform.localScale = Vector3.zero;
        newNoteObject.DrawTool = DrawTool;
    }

    void PerformManipulationUpdate(Vector3 position)
    {
        if (GestureManager.Instance.manipulationTarget != null)
        {
            Vector3 pos1 = gameObject.transform.InverseTransformPoint(startPosition);
            Vector3 pos2 = gameObject.transform.InverseTransformPoint(position);
            float scalex = Mathf.Abs(pos2.x - pos1.x);
            float scaley = Mathf.Abs(pos2.y - pos1.y);
            gameObject.transform.position = (startPosition + position) / 2;
            gameObject.transform.LookAt(Camera.main.transform);
            newQuad.transform.localScale = new Vector3(scalex, scaley, 1);
            Debug.Log("Scale (x, y) = (" + scalex + ", " + scaley + ")");
        }
    }

    void PerformManipulationCompleted()
    {
        NoteManager.Instance.SetActiveNote(newNoteObject);
        newNoteObject.gameObject.transform.SetParent(Notes, true);
        DrawTool.Activate();
    }

    void PerformManipulationCanceled()
    {
        Destroy(newNoteObject);
    }
}
