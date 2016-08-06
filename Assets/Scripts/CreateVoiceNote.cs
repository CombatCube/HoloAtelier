using UnityEngine;
using HoloToolkit.Unity;

public class CreateVoiceNote : Tool {

    public VoiceNote VoiceNotePrefab;
    private VoiceNote newVoiceNoteObj;
    private const float planeOffset = 0.02f;
    private bool startedRecording;
    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (!startedRecording)
        {
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.enabled = true;
            }
            gameObject.transform.position = GazeManager.Instance.Position + planeOffset * (-Camera.main.transform.forward);
            gameObject.transform.LookAt(Camera.main.transform, Vector3.up);
        }
        else
        {
            if (NoteManager.Instance.recording)
            {
                foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
                {
                    mesh.enabled = false;
                }
            }
            else
            {
                startedRecording = false;
                newVoiceNoteObj = null;
                ToolManager.Instance.SetActiveTool(null);
            }
        }
    }

    void OnSelect()
    {
        // Create new voice note. Next tap stops recording.
        if (newVoiceNoteObj == null)
        {
            startedRecording = true;
            newVoiceNoteObj = NoteManager.Instance.CreateVoiceNote(
                "",
                NoteManager.Instance.transform.InverseTransformPoint(transform.position),
                Quaternion.Inverse(NoteManager.Instance.transform.rotation) * transform.rotation
            );
            newVoiceNoteObj.Record();
        }
        else if (NoteManager.Instance.recording)
        {
            newVoiceNoteObj.RecordStop();
        }
    }
}
