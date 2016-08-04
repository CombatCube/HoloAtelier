using UnityEngine;
using HoloToolkit.Unity;

public class CreateVoiceNote : Tool {

    public VoiceNote VoiceNotePrefab;
    private VoiceNote newVoiceNoteObj;
    private const float planeOffset = 0.02f;
    private bool recording = false;
    // Use this for initialization
    void Start () {
	
	}

    // Update is called once per frame
    void Update()
    {
        if (!recording)
        {
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
            {
                mesh.enabled = true;
            }
            gameObject.transform.position = GazeManager.Instance.Position + planeOffset * (-Camera.main.transform.forward);
            gameObject.transform.LookAt(Camera.main.transform);
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
        // Create new voice note. Next tap stops recording.
        if (newVoiceNoteObj == null)
        {
            recording = true;
            newVoiceNoteObj = NoteManager.Instance.CreateVoiceNote(
                CustomMessages.Instance.localUserID,
                NoteManager.Instance.transform.InverseTransformPoint(transform.position),
                Quaternion.Inverse(NoteManager.Instance.transform.rotation) * transform.rotation
            );
            newVoiceNoteObj.Record();
        }
        else if (recording)
        {
            recording = false;
            newVoiceNoteObj.RecordStop();
            AudioClip clip = newVoiceNoteObj.GetComponent<AudioSource>().clip;
            float[] data = new float[clip.samples * clip.channels];
            clip.GetData(data, 0);
            CustomMessages.Instance.SendVoiceNote(
                newVoiceNoteObj.GetInstanceID(),
                (byte)Note.NoteType.Voice,
                newVoiceNoteObj.transform.localPosition,
                newVoiceNoteObj.transform.localRotation,
                newVoiceNoteObj.microphoneManager.DictationDisplay.text,
                data
            );
            newVoiceNoteObj = null;
            ToolManager.Instance.SetActiveTool(null);
        }
    }
}
