using UnityEngine;
using HoloToolkit.Unity;

public class Create2DNote : Tool {

    public Note NoteObject2D;
    public Draw3D DrawTool;
    public Transform Notes;
    
    private Vector3 startPosition;
    public MeshRenderer QuadObject;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
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
                gameObject.transform.LookAt(Camera.main.transform);
            }
        }
        else
        {
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.enabled = false;
            }
        }
    }


    void PerformManipulationStart(Vector3 position)
    {
        startPosition = position;
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
            QuadObject.transform.localScale = new Vector3(scalex, scaley, 1);
        }
    }

    void PerformManipulationCompleted()
    {
        Debug.Log("Quad scale = " + QuadObject.transform.localScale);
        Note note = NoteManager.Instance.CreateNewNote(
            CustomMessages.Instance.localUserID,
            (byte)DrawCanvas.DrawMode.Draw2D,
            NoteManager.Instance.transform.InverseTransformPoint(transform.position),
            Quaternion.Inverse(NoteManager.Instance.transform.rotation) * transform.rotation,
            QuadObject.transform.localScale
        );
        QuadObject.transform.localScale = Vector3.zero;
        NoteManager.Instance.SetActiveNote(note);
        DrawTool.Activate();
    }

    void PerformManipulationCanceled()
    {
        QuadObject.transform.localScale = Vector3.zero;
    }
}
