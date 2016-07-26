using UnityEngine;
using System.Collections;

public class NoteHandle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnSelect()
    {
        GetComponentInParent<Note>().OnSelect();
    }
}
