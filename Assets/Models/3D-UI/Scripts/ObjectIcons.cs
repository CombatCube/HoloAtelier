using UnityEngine;
using System;
using System.Collections;

public class ObjectIcons : MonoBehaviour {

    float originalY;
    public float floatStrength = 0.025f;
    bool mouseEnter = false;
    bool mouseClicked = false;
    bool clickToFoldChildren = false;
    
    public static GameObject currentIcon;

    public GameObject chatBubble;
    public GameObject childPanel;
    
    Color objectColor;

    void Start()
    {
        this.originalY = this.transform.position.y;
        objectColor = this.GetComponent<MeshRenderer>().material.color;
        chatBubble.SetActive(false);
        //defaultScale = this.transform.localScale;

        //this.transform.localScale = new Vector3(0, 0, 0);
    }

    //float sizeUpDelta;
    //float sizeUpLimit;
    //Vector3 defaultScale;
    void FixedUpdate()
    {
        this.transform.position = new Vector3(transform.position.x, originalY + ((float)Math.Sin(Time.time * 2) * floatStrength), transform.position.z);
        /*if (GlobalSwitch.uiSetVisible)
         {
             if (sizeUpDelta <= defaultScale.x) {
                 this.transform.localScale += new Vector3(defaultScale.x*0.1f, defaultScale.y * 0.1f, defaultScale.z * 0.1f);
                 sizeUpDelta = transform.localScale.x - defaultScale.x * 0.1f;
             }
         }
         else {
             this.transform.localScale = new Vector3(0, 0, 0);           
         }*/

       /* if (GlobalSwitch.uiSetVisible) {
            this.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
        }*/

    }

    float lastTime;
    float timeLimit = 2;

    void OnMouseOver() {           
        mouseEnter = true;
        if ((Time.time > lastTime + 1) && timeLimit > 0)
        {
                lastTime = Time.time;
                timeLimit--;
        }
        if (timeLimit == 0)
            {
                timeLimit = 0;
            chatBubble.SetActive(true);
        }
        
    }

    void OnMouseExit()
    {
        mouseEnter = false;
        chatBubble.SetActive(false);
        timeLimit = 2;

    }

    void OnMouseUp() {
        if (currentIcon != this.gameObject)
        {
            currentIcon = this.gameObject;
            mouseClicked = true;
        }
        else {
            clickToFoldChildren = true;
        }       
               
    }

    void Update() {
        if (currentIcon == this.gameObject) {
            if (!clickToFoldChildren)
            {
                this.GetComponent<MeshRenderer>().material.color = objectColor;
                objectColor.a = 0.1f;
                this.GetComponent<MeshRenderer>().material.color = objectColor;
                chatBubble.SetActive(false);
                childPanel.SetActive(true);
            }
            else {
                clickToFoldChildren = false;
                currentIcon = null;               
            }        
        }
        else{
            childPanel.SetActive(false);
            if (!mouseEnter)
            {
                objectColor.a = 0.75f;
                this.GetComponent<MeshRenderer>().material.color = objectColor;
            }
            else {
                this.GetComponent<MeshRenderer>().material.color = Color.yellow;
            }                     
        }       
    }
}
