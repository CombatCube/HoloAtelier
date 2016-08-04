using UnityEngine;
using System.Collections;

public class NoteHandle : MonoBehaviour {

    private Note note;
    private bool spin = false;
    private float frequency = 2 * Mathf.PI;
    private float magnitude = 0.005f;
    private float rotationSpeed = 180.0f; //degrees per second
	// Use this for initialization
	void Start () {
        note = GetComponentInParent<Note>();
    }
	
	// Update is called once per frame
	void Update () {
        if (spin)
        {
            transform.Rotate(Vector3.up, rotationSpeed*Time.deltaTime);
        } else
        {

        }
        if (note == NoteManager.Instance.ActiveNote)
        {
            transform.Translate(new Vector3(0f, Mathf.Sin(Time.time * frequency) * magnitude, 0f));
        }
	}

    void OnSelect()
    {
        if (note is VoiceNote)
        {
            GetComponentInParent<VoiceNote>().OnSelect();
        } else
        {
            note.OnSelect();
        }
    }

    void OnGazeEnter()
    {
        Debug.Log("Gaze entered.");
        spin = true;
    }

    void OnGazeLeave()
    {
        Debug.Log("Gaze exited.");
        spin = false;
    }
}
