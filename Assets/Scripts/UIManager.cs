using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    public GameObject Menu1;
    public GameObject Menu2;
    public GameObject Menu3;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnMenu1()
    {
        Menu1.SetActive(true);
        Menu2.SetActive(false);
        Menu3.SetActive(false);
    }
    void OnMenu2()
    {
        Menu1.SetActive(false);
        Menu2.SetActive(true);
        Menu3.SetActive(false);
    }
    void OnMenu3()
    {
        Menu1.SetActive(false);
        Menu2.SetActive(false);
        Menu3.SetActive(true);
    }
}
