﻿using UnityEngine;
using System.Collections;

public class ChildrenIcons : MonoBehaviour {
    Color objectColor;
    //public GameObject learningAids;
    //public static GameObject currentChild;

    void Start()
    {    
        objectColor = this.GetComponent<MeshRenderer>().material.color;
    }

    void OnGazeEnter()
    {
        this.GetComponent<MeshRenderer>().material.color = Color.yellow;
    }

    void OnGazeLeave()
    {
        this.GetComponent<MeshRenderer>().material.color = objectColor;
    }

    /*
    void OnMouseUp() {
        currentChild = this.gameObject;
        learningAids.SetActive(true);        
    }

    void Update() {
        if (currentChild != this.gameObject) {
            learningAids.SetActive(false);
        }        
    }*/
}
