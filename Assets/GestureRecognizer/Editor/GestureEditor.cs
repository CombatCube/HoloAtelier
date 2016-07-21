using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace GestureRecognizer
{
    public class GestureEditor : EditorWindow, IHasCustomMenu
    {
        private Event e {
            get {
                return Event.current;
            }
        }


        /// <summary>
        /// The library that is currently being edited.
        /// </summary>
        private GestureLibrary currentLibrary;

        /// <summary>
        /// The gesture that is currently being edited.
        /// </summary>
        private Gesture currentGesture;

        /// <summary>
        /// EditorView / LibraryView ratio.
        /// </summary>
        private float viewRatio = 0.8f;

        /// <summary>
        /// The editor view.
        /// </summary>
        private Rect editorView;

        /// <summary>
        /// The library view.
        /// </summary>
        private Rect libraryView;

        /// <summary>
        /// The area that limits gesture drawing.
        /// </summary>
        private Rect gestureDrawArea;

        /// <summary>
        /// The resizer.
        /// </summary>
        private Rect resizer;

        /// <summary>
        /// Are views being resized.
        /// </summary>
        private bool isResizing;

        /// <summary>
        /// Is the user adding a gesture.
        /// </summary>
        private bool isAddingGesture;

        /// <summary>
        /// Newly added gesture name
        /// </summary>
        private string newGestureName;

        /// <summary>
        /// Original name of the gesture.
        /// </summary>
        private string originalGestureName;

        /// <summary>
        /// Points of the new gesture.
        /// </summary>
        private List<Point> points;

        /// <summary>
        /// Stroke ID of strokes.
        /// </summary>
        private int strokeID = -1;

        /// <summary>
        /// Last added point.
        /// </summary>
        private Vector2 lastPoint;

        /// <summary>
        /// Is mouse dragged.
        /// </summary>
        private bool isDrawing = false;

        /// <summary>
        /// Is the editor window locked
        /// </summary>
        private bool isLocked = false;

        /// <summary>
        /// Is renaming current gesture.
        /// </summary>
        private bool isRenamingCurrentGesture = false;

        /// <summary>
        /// Is the name edit field newly opened.
        /// </summary>
        private bool isNameEditFieldNewlyOpened = false;

        /// <summary>
        /// Context menu to show when a library is not being edited.
        /// </summary>
        private GenericMenu libraryListMenu;

        /// <summary>
        /// Context menu to show when a library is being edited.
        /// </summary>
        private GenericMenu gestureListMenu;

        /// <summary>
        /// Context menu to show on right click to a gesture.
        /// </summary>
        private GenericMenu gestureMenu;

        /// <summary>
        /// The resizer style.
        /// </summary>
        private GUIStyle resizerStyle;

        /// <summary>
        /// Current gesture button style.
        /// </summary>
        private GUIStyle gestureStyle;

        /// <summary>
        /// Lock button style.
        /// </summary>
        private GUIStyle lockButtonStyle;

        /// <summary>
        /// Editor skin.
        /// </summary>
        private GUISkin skin;

		/// <summary>
		/// Scroll vector for gesture list
		/// </summary>
		private Vector2 gestureListScroll;


        [MenuItem("Window/Gesture Editor")]
        public static void Init()
        {
            GestureEditor window = GetWindow<GestureEditor>();
            window.titleContent = new GUIContent("Gesture Editor");
        }


        public void Initialize()
        {
            currentLibrary = null;
            currentGesture = null;

            libraryListMenu = new GenericMenu();
            gestureListMenu = new GenericMenu();
            gestureMenu = new GenericMenu();

            libraryListMenu.AddItem(new GUIContent("Add gesture library"), false, OnClickAddGestureLibrary);
            gestureListMenu.AddItem(new GUIContent("Add gesture"), false, OnClickAddGesture);
            gestureMenu.AddItem(new GUIContent("Remove"), false, OnClickRemoveGesture);
            gestureMenu.AddItem(new GUIContent("Rename"), false, OnClickRenameGesture);

            resizerStyle = new GUIStyle();
            resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

			if (EditorGUIUtility.isProSkin)
			{
				skin = EditorGUIUtility.Load("GUISkins/GestureEditorProSkin.guiskin") as GUISkin;
			}
			else
			{
				skin = EditorGUIUtility.Load("GUISkins/GestureEditorSkin.guiskin") as GUISkin;
			}

			newGestureName = "";
            isAddingGesture = false;
            points = new List<Point>();
            lastPoint = Vector2.zero;
            strokeID = -1;
        }


        private void OnEnable()
        {
            Initialize();
            SetCurrentLibrary();
        }


        private void OnSelectionChange()
        {
            SetCurrentLibrary();
        }


        private void ShowButton(Rect position)
        {
            if (lockButtonStyle == null)
                lockButtonStyle = "IN LockButton";
            isLocked = GUI.Toggle(position, isLocked, GUIContent.none, lockButtonStyle);
        }


        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Lock"), isLocked, () => {
                isLocked = !isLocked;
            });
        }


        private void OnGUI()
        {
            ProcessRenaming(Event.current);

            OnGUIEditorView();
            OnGUILibraryView();
            OnGUIResizer();

            ProcessEvent(Event.current);
            Resize(Event.current);
            Repaint();
        }


        private void OnGUIEditorView()
        {
            editorView = new Rect(0, 0, Mathf.FloorToInt(position.width * viewRatio), position.height);

            GUILayout.BeginArea(editorView, EditorStyles.inspectorFullWidthMargins);

            if (isAddingGesture)
            {
                DrawGestureArea();
                DrawInstructions();
                GestureEditorUtility.DrawPointList(points, gestureDrawArea);
            }
            else
            {
                if (currentGesture != null)
                {
                    DrawGestureArea();
                    GestureEditorUtility.DrawGesture(currentGesture, gestureDrawArea);
                }
            }

            GUILayout.EndArea();
        }


        private void OnGUILibraryView()
        {
            libraryView = new Rect(Mathf.FloorToInt(position.width * viewRatio), 0, Mathf.FloorToInt(position.width * (1 - viewRatio)), position.height);

            GUILayout.BeginArea(libraryView);
			gestureListScroll = GUILayout.BeginScrollView(gestureListScroll);

            if (currentLibrary != null)
            {
                DrawGestureList();
            }

			GUILayout.EndScrollView();
            GUILayout.EndArea();
        }


        private void OnGUIResizer()
        {
            resizer = new Rect(libraryView.xMin - 10, 0, 10, position.height);

            GUILayout.BeginArea(new Rect(resizer.position + (Vector2.right * 9), new Vector2(1, position.height)), resizerStyle);
            GUILayout.EndArea();

            EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeHorizontal);
        }


        private void DrawGestureList()
        {
			GUILayout.Label(currentLibrary.name, EditorStyles.boldLabel);

            for (int i = 0; i < currentLibrary.Gestures.Count; i++)
            {
                if (currentGesture == currentLibrary.Gestures[i])
                {
                    gestureStyle = skin.GetStyle("GestureSelected");
                }
                else
                {
                    gestureStyle = skin.GetStyle("Gesture");
                }

                if (isRenamingCurrentGesture && currentGesture == currentLibrary.Gestures[i])
                {
                    newGestureName = currentLibrary.Gestures[i].Name;
                    GUI.SetNextControlName("NameEdit");
                    newGestureName = EditorGUILayout.TextField(newGestureName, skin.GetStyle("GestureTextField"));
                    currentLibrary.Gestures[i].Name = newGestureName;

                    if (isNameEditFieldNewlyOpened) {
                        isNameEditFieldNewlyOpened = false;
                        EditorGUI.FocusTextInControl("NameEdit");
                    }
                }
                else 
                {
                    GUI.SetNextControlName("Button");
                    if (GUILayout.Button(currentLibrary.Gestures[i].Name, gestureStyle))
                    {
                        currentGesture = currentLibrary.Gestures[i];

                        if (e.button == 1)
                        {
                            newGestureName = "";
                            GUI.FocusControl("Button");
                            ProcessContextMenu();
                        }
                    }
                }
            }
        }


        private void DrawGestureArea()
        {
            gestureDrawArea = new Rect(Mathf.FloorToInt((editorView.width - 256) * 0.5f), Mathf.FloorToInt((editorView.height - 256) * 0.5f), 256, 256);
            GUI.Box(gestureDrawArea, "");
        }


        private void DrawInstructions()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Draw your gesture, enter a name in the text field and then click 'Add'");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            newGestureName = GUILayout.TextField(newGestureName, 32, GUILayout.Width(200));

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (newGestureName == "")
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Add", GUILayout.Width(100)))
            {
                Gesture g = new Gesture(points.ToArray(), newGestureName);
                currentLibrary.Gestures.Add(g);
                ClearGesture();
                isAddingGesture = false;
				EditorUtility.SetDirty(currentLibrary);
            }

            GUI.enabled = true;

            if (points.Count == 0)
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Clear", GUILayout.Width(100)))
            {
                ClearGesture();
				EditorUtility.SetDirty(currentLibrary);
			}

            GUI.enabled = true;

            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
            {
                ClearGesture();
                isAddingGesture = false;
				EditorUtility.SetDirty(currentLibrary);
			}

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        private void Resize(Event e)
        {
            if (!isDrawing && isResizing)
            {
                viewRatio = e.mousePosition.x / position.width;
            }
        }


        private void ProcessRenaming(Event e) {
            if (isRenamingCurrentGesture) 
            {
                if (e.type == EventType.KeyUp)
                {
                    if (e.keyCode == KeyCode.Return)
                    {
                        isRenamingCurrentGesture = false;
                        isNameEditFieldNewlyOpened = false;
						EditorUtility.SetDirty(currentLibrary);
                    }
                    else if (e.keyCode == KeyCode.Escape)
                    {
                        isRenamingCurrentGesture = false;
                        isNameEditFieldNewlyOpened = false;
                        currentGesture.Name = originalGestureName;
						EditorUtility.SetDirty(currentLibrary);
					}
                } 
                else if (e.type == EventType.mouseDown) 
                {
                    isRenamingCurrentGesture = false;
                    isNameEditFieldNewlyOpened = false;
                }
            }
        }


        private void ProcessEvent(Event e)
        {
            switch (e.type)
            {
                case EventType.ContextClick:

                    if (libraryView.Contains(e.mousePosition) || editorView.Contains(e.mousePosition))
                    {
						if (!isAddingGesture)
						{
							ProcessContextMenu(); 
						}
                    }

                    break;

                case EventType.MouseDown:

                    if (e.button == 0)
                    {
                        if (resizer.Contains(e.mousePosition))
                        {
                            isResizing = true;
                        }

                        if (libraryView.Contains(e.mousePosition) || editorView.Contains(e.mousePosition))
                        {
                            currentGesture = null;
                        }

                        if (gestureDrawArea.Contains(e.mousePosition))
                        {
                            isDrawing = true;
                            ++strokeID;
                            CapturePoint(e.mousePosition);
                        }
                    }

                    break;

                case EventType.MouseUp:
                    isResizing = false;
                    isDrawing = false;
                    isRenamingCurrentGesture = false;
                    isNameEditFieldNewlyOpened = false;
                    break;

                case EventType.MouseDrag:

                    if (isDrawing && gestureDrawArea.Contains(e.mousePosition) && Vector2.Distance(lastPoint, e.mousePosition) > 5f)
                    {
                        CapturePoint(e.mousePosition);
                    }

                    break;
            }
        }


        private void ProcessContextMenu()
        {
            if (currentLibrary == null)
            {
                libraryListMenu.ShowAsContext();
            }
            else
            {
                if (currentGesture == null)
                {
                    gestureListMenu.ShowAsContext();   
                }
                else
                {
                    gestureMenu.ShowAsContext();
                }
            }
        }


        private void OnClickAddGestureLibrary()
        {

        }


        private void OnClickAddGesture()
        {
            ClearGesture();
            isAddingGesture = true;
        }


        private void OnClickRemoveGesture()
        {
            currentLibrary.Gestures.Remove(currentGesture);
            currentGesture = null;
			EditorUtility.SetDirty(currentLibrary);
        }


        private void OnClickRenameGesture()
        {
            isRenamingCurrentGesture = true;
            isNameEditFieldNewlyOpened = true;
            originalGestureName = currentGesture.Name;
        }


        private void CapturePoint(Vector2 point)
        {
            Point p = new Point(strokeID, TranslateToDrawArea(point));
            lastPoint = point;
            points.Add(p);
        }


        private void ClearGesture()
        {
            newGestureName = "";
            points = new List<Point>();
            strokeID = -1;
        }
            

        private void SetCurrentLibrary()
        {
            if (Selection.activeObject != null && Selection.activeObject.GetType() == typeof(GestureLibrary))
            {
                currentLibrary = (GestureLibrary)Selection.activeObject;
                currentGesture = null;
            }
            else
            {
                if (!isLocked)
                {
                    currentLibrary = null;
                    currentGesture = null;
                }
            }
        }


        private Vector2 TranslateToDrawArea(Vector2 point, bool additive = false)
        {
            return point + (additive ? gestureDrawArea.position : -gestureDrawArea.position);
        }
    }
}