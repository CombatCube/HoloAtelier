using UnityEngine;

public class ModelManager : MonoBehaviour {

    public GameObject ActiveModel;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ToggleLayer(string name)
    {
        Transform layer = ActiveModel.transform.Find(name);
        if (layer != null)
        {
            layer.gameObject.SetActive(!layer.gameObject.activeSelf);
        }
        else
        {
            Debug.Log("Cannot toggle active of " + name);
        }
    }
}
