using UnityEngine;
using System.Collections;

public class GlobalSwitch : MonoBehaviour {

    public GameObject uiSet;
    public static bool uiSetVisible;

    void Start() {
        uiSet.SetActive(false);
    }

    void OnMouseUp() {
        if (!uiSet.activeInHierarchy)
        {
            uiSetVisible = true;
            uiSet.SetActive(true);
            
        }
        else {
            uiSetVisible = false;
            uiSet.SetActive(false);
        }
        
    }
}
