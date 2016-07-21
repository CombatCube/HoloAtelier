using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class ShowScreenRect : MonoBehaviour {

    public RectTransform rectTransform;

    private ScreenRect r;
    private Canvas canvas;

    void Awake() {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

#if UNITY_EDITOR
    public void OnGUI() {
        r = rectTransform.GetScreenRect(canvas);

        Handles.BeginGUI();
        Handles.DrawLine(new Vector3(r.xMin, r.yMin, 0), new Vector3(r.xMax, r.yMin, 0));
        Handles.DrawLine(new Vector3(r.xMax, r.yMin, 0), new Vector3(r.xMax, r.yMax, 0));
        Handles.DrawLine(new Vector3(r.xMax, r.yMax, 0), new Vector3(r.xMin, r.yMax, 0));
        Handles.DrawLine(new Vector3(r.xMin, r.yMax, 0), new Vector3(r.xMin, r.yMin, 0));

        Handles.EndGUI();
    }
#endif

}
