using UnityEngine;
using System.Collections;

public class SelfRotate : MonoBehaviour {

    public float speed = 5;

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, speed * Time.deltaTime * 10, 0);
    }
}
