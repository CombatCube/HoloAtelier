using UnityEngine;
using UnityEditor;
using GestureRecognizer;

[CustomEditor(typeof(GestureLibrary))]
public class GestureLibraryInspector : Editor
{
    public override void OnInspectorGUI()
    {
        
        GestureLibrary library = (GestureLibrary)target;
        
        if (GUILayout.Button("Open in Gesture Editor"))
        {
            GestureEditor.Init();
        }

		if (GUILayout.Button("Resample Gestures"))
		{
			library.ResampleGestures();
		}

		GUILayout.BeginHorizontal();

		if (GUILayout.Button("Expand All"))
		{
			for (int i = 0; i < library.Gestures.Count; i++)
			{
				library.Gestures[i].IsShown = true;
			}
		}

		if (GUILayout.Button("Collapse All"))
		{
			for (int i = 0; i < library.Gestures.Count; i++)
			{
				library.Gestures[i].IsShown = false;
			}
		}

		GUILayout.EndHorizontal();

        for (int i = 0; i < library.Gestures.Count; i++)
        {
            library.Gestures[i].IsShown = EditorGUILayout.Foldout(library.Gestures[i].IsShown, library.Gestures[i].Name);

            if (library.Gestures[i].IsShown)
            {
                GUILayout.Box("", GUILayout.Width(256), GUILayout.Height(256));
                GestureEditorUtility.DrawGesture(library.Gestures[i], GUILayoutUtility.GetLastRect());
            }
        }
    }
}