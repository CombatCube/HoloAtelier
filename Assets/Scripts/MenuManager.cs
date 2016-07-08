using UnityEngine;

public class MenuManager : MonoBehaviour {

	public GameObject ActiveMenu;


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void ToggleMenu(string name)
	{

		if (name == "Menu 1") {
			Transform menu = ActiveMenu.transform.Find (name);
			Transform InactiveMenu = ActiveMenu.transform.Find ("Menu 2");
			InactiveMenu.gameObject.SetActive (false);
			menu.gameObject.SetActive(!menu.gameObject.activeSelf);
		}
		else
		if (name == "Menu 2") {
			Transform menu = ActiveMenu.transform.Find (name);
			Transform InactiveMenu = ActiveMenu.transform.Find ("Menu 1");
			InactiveMenu.gameObject.SetActive (false);
			menu.gameObject.SetActive(!menu.gameObject.activeSelf);
		}


	}
}

