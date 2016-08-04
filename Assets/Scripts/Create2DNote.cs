using UnityEngine;
using HoloToolkit.Unity;

public class Create2DNote : Tool {

    public Note NoteObject2D;
    public Draw3D DrawTool;
    public Transform Notes;
    
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
        Note note = NoteManager.Instance.CreateNewNote(
            SystemInfo.deviceUniqueIdentifier + "-" + GetInstanceID(),
            (byte)Note.NoteType.Draw2D,
            NoteManager.Instance.transform.InverseTransformPoint(transform.position),
            Quaternion.Inverse(NoteManager.Instance.transform.rotation) * transform.rotation
        );
        NoteManager.Instance.SetActiveNote(note);
        DrawTool.Activate();
    }
}
