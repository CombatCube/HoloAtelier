using UnityEngine;
using HoloToolkit.Unity;

public class Create2DNote : Tool {

    public Note NoteObject2D;
    public Draw3D DrawTool;
    public Transform Notes;
    
    private Vector3 startPosition;
    public MeshRenderer QuadObject;
    private const float planeOffset = 0.02f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.enabled = true;
        }
        gameObject.transform.position = GazeManager.Instance.Position + planeOffset * (-Camera.main.transform.forward);
        gameObject.transform.LookAt(Camera.main.transform);
    }

    void OnSelect()
    {
        Debug.Log("Quad scale = " + QuadObject.transform.localScale);
        Note note = NoteManager.Instance.CreateNewNote(
            CustomMessages.Instance.localUserID,
            (byte)Note.NoteType.Draw2D,
            NoteManager.Instance.transform.InverseTransformPoint(transform.position),
            Quaternion.Inverse(NoteManager.Instance.transform.rotation) * transform.rotation,
            QuadObject.transform.localScale
        );
        NoteManager.Instance.SetActiveNote(note);
        DrawTool.Activate();
    }
}
