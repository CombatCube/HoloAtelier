using UnityEngine;
using HoloToolkit.Unity;

public class Create3DNote : Tool {

    public Note NoteObject3D;
    public Draw3D DrawTool;
    public Transform Notes;

	// Use this for initialization
	void Start () {
	
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

    void OnSelect()
    {
        Note note = NoteManager.Instance.CreateNewNote(
            CustomMessages.Instance.localUserID,
            (byte)Note.NoteType.Draw3D,
            NoteManager.Instance.transform.InverseTransformPoint(transform.position),
            Quaternion.Inverse(NoteManager.Instance.transform.rotation) * transform.rotation
        );
        NoteManager.Instance.SetActiveNote(note);
        DrawTool.Activate();
    }

}
