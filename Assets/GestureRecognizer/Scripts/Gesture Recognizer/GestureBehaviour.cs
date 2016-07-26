using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using HoloToolkit.Unity;

namespace GestureRecognizer
{
	public enum GestureLimitType
    {
        /// <summary>
        /// Drawing is not limited in any way.
        /// </summary>
        Unlimited,

        /// <summary>
        /// Check if the drawing started in an area. Can draw outside of the area.
        /// </summary>
        StartInArea,

        /// <summary>
        /// Clamps the drawing in an area.
        /// </summary>
        ClampToArea,

        /// <summary>
        /// Ignores points outside area.
        /// </summary>
        IgnoreOutside
    }

    public class GestureBehaviour : MonoBehaviour
    {

        /// <summary>
        /// Disable or enable multi stroke recognition
        /// </summary>
        public bool isEnabled = true;

        /// <summary>
        /// Loaded multiStroke library.
        /// </summary>
        public GestureLibrary library;

        /// <summary>
        /// How to limit drawing.
        /// </summary>
        public GestureLimitType limitType = GestureLimitType.Unlimited;

        /// <summary>
        /// A new point will be placed if it is this further than the last point.
        /// </summary>
        public float distanceBetweenPoints = 0.01f;

        /// <summary>
        /// Minimum amount of points required to recognize a multistroke.
        /// </summary>
        public int minimumPointsToRecognize = 10;

        /// <summary>
        /// Material for the line renderer.
        /// </summary>
        public Material lineMaterial;

        /// <summary>
        /// Start thickness of the multi stroke.
        /// </summary>
        public float startThickness = 0.5f;

        /// <summary>
        /// End thickness of the multi stroke.
        /// </summary>
        public float endThickness = 0.05f;

        /// <summary>
        /// Start color of the multi stroke.
        /// </summary>
        public Color startColor = Color.red;

        /// <summary>
        /// End color of the multi stroke.
        /// </summary>
        public Color endColor = Color.white;

        /// <summary>
        /// Line renderer component.
        /// </summary>
        private LineRenderer currentStrokeRenderer;

        /// <summary>
        /// Last stroke's ID.
        /// </summary>
        private int currentStrokeID = -1;

        /// <summary>
        /// Last added point.
        /// </summary>
        private Vector2 lastPoint = Vector2.zero;

        /// <summary>
        /// Vertex count of the line renderer.
        /// </summary>
        private int vertexCount = 0;

        /// <summary>
        /// Captured points
        /// </summary>
        private List<Point> points = new List<Point>();

        /// <summary>
        /// Strokes.
        /// </summary>
        private List<GameObject> strokes = new List<GameObject>();

        /// <summary>
        /// The gesture.
        /// </summary>
        private Gesture gesture;

        /// <summary>
        /// Is the gesture recognized.
        /// </summary>
        private bool isRecognized = false;

        ///// <summary>
        ///// Screen rect of draw area.
        ///// </summary>
        //private ScreenRect limitedDrawAreaRect;

        ///// <summary>
        ///// Unlimited draw area.
        ///// </summary>
        //private RectTransform unlimitedDrawArea;

        ///// <summary>
        ///// Limited draw area.
        ///// </summary>
        //private RectTransform limitedDrawArea;

        ///// <summary>
        ///// The canvas.
        ///// </summary>
        //private Canvas canvas;

        /// <summary>
        /// "Screen point" of a point. Since GR records gestures on editor coordinates (which
        /// puts (0,0) on top-left), we need to revert captured point from screen coordinates
        /// (which puts (0,0) on bottom left) to editor coordinates.
        /// </summary>
        private Vector3 screenPoint;
        

        /// <summary>
        /// Occurs when a gesture is recognized.
        /// </summary>
        public static event Action<Gesture, Result> OnGestureRecognition;


        private void Start()
        {
   //         canvas = GetComponentInChildren<Canvas>();
   //         unlimitedDrawArea = transform.Find("Canvas/UnlimitedDrawArea").GetComponent<RectTransform>();
   //         limitedDrawArea = transform.Find("Canvas/LimitedDrawArea").GetComponent<RectTransform>();

   //         if (limitType == GestureLimitType.Unlimited)
   //         {
   //             limitedDrawArea.gameObject.SetActive(false);
   //         }
   //         else
   //         {
   //             unlimitedDrawArea.gameObject.SetActive(false);
			//	limitedDrawAreaRect = limitedDrawArea.GetScreenRect(canvas);
			//}
        }

        // Update is called once per frame
        void Update()
        {
            if (HandsManager.Instance.HandDetected)
            {
                GetComponentInChildren<MeshRenderer>().enabled = true;

                Vector3 pos;
                HandsManager.Instance.Hand.properties.location.TryGetPosition(out pos);
                // Move tool to hand position until hand starts dragging
                if (GestureManager.Instance.manipulationTarget == null)
                {
                    gameObject.transform.position = pos;
                    gameObject.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Camera.main.transform.up);
                }
                // Lock tool's hand position until finished dragging
            }
            else
            {
                GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }

        /// <summary>
        /// Add a new stroke to multi stroke
        /// </summary>
        private void CreateNewStroke(Vector2 point)
        {
            //if (limitType != GestureLimitType.Unlimited && !limitedDrawAreaRect.Contains(point))
            //{
            //    return;
            //}

            if (isRecognized)
            {
                ClearGesture();
            }

            lastPoint = Vector2.zero;
            currentStrokeID++;
            vertexCount = 0;

            GameObject newStroke = new GameObject();
            newStroke.name = "Stroke " + currentStrokeID;
            newStroke.transform.SetParent(this.transform, false);

            currentStrokeRenderer = newStroke.AddComponent<LineRenderer>();
            currentStrokeRenderer.useWorldSpace = false;
            currentStrokeRenderer.SetVertexCount(0);
            currentStrokeRenderer.material = lineMaterial;
            currentStrokeRenderer.SetColors(startColor, endColor);
            currentStrokeRenderer.SetWidth(startThickness, endThickness);

            strokes.Add(newStroke);

            RegisterPoint(point);
        }

        private void RegisterPoint(Vector3 point)
        {
            if (Vector2.Distance(point, lastPoint) > distanceBetweenPoints)
            {
                points.Add(new Point(currentStrokeID, -point.x, -point.y));
                lastPoint = point;
                vertexCount++;
                currentStrokeRenderer.SetVertexCount(vertexCount);
                currentStrokeRenderer.SetPosition(vertexCount - 1, point);
            }
        }

		/// <summary>
		/// Recognize the specified gesture.
		/// </summary>
		/// <param name="gesture">Gesture.</param>
		private void Recognize()
        {
            if (points.Count > 2)
            {
                Gesture gesture = CreateGesture();
                Result result = library.Recognize(gesture);
                
                isRecognized = true;
                
                if (OnGestureRecognition != null)
                    OnGestureRecognition(gesture, result);
            }
            ClearGesture();
        }


        /// <summary>
        /// Creates the gesture.
        /// </summary>
        private Gesture CreateGesture()
        {
            return new Gesture(points.ToArray());
        }


        /// <summary>
        /// Remove the multi stroke from the screen.
        /// </summary>
        public void ClearGesture()
        {
            vertexCount = 0;
            currentStrokeID = -1;
            points.Clear();

            for (int i = strokes.Count - 1; i >= 0; i--)
            {
                Destroy(strokes[i]);
            }

            strokes.Clear();
            isRecognized = false;
        }

        void PerformManipulationStart(Vector3 position)
        {
            Vector3 localPos = gameObject.transform.InverseTransformPoint(position);
            Vector3 planePos = Vector3.ProjectOnPlane(localPos, Vector3.forward);
            CreateNewStroke(planePos);
        }

        void PerformManipulationUpdate(Vector3 position)
        {
            Vector3 localPos = gameObject.transform.InverseTransformPoint(position);
            Vector3 planePos = Vector3.ProjectOnPlane(localPos, Vector3.forward);
            RegisterPoint(planePos);
        }

        void PerformManipulationCompleted()
        {
            Recognize();
        }

        void PerformManipulationCanceled()
        {
            Recognize();
        }

        public void Activate()
        {
            if (ToolManager.Instance.ActiveTool != gameObject)
            {
                ToolManager.Instance.SetActiveTool(gameObject);
            }
            else
            {
                ToolManager.Instance.SetActiveTool(null);
            }
        }
    }
}