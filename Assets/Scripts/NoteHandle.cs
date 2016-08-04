using UnityEngine;
using System.Collections;

public class NoteHandle : MonoBehaviour {

    private Note note;
    private bool spin = false;
	// Use this for initialization
	void Start () {
        note = GetComponentInParent<Note>();
    }
	
	// Update is called once per frame
	void Update () {
        if (spin)
        {
            transform.Rotate(Vector3.up, 1 * Time.deltaTime);
        }
	}

    void OnSelect()
    {
        GetComponentInParent<Note>().OnSelect();
    }

    void OnGazeEnter()
    {
        spin = true;
    }

    void OnGazeLeave()
    {
        spin = false;
    }
}
