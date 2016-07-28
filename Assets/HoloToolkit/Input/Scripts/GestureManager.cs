// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager creates a gesture recognizer and signs up for a tap gesture.
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// GestureManager then sends a message to that game object.
    /// </summary>
    [RequireComponent(typeof(GazeManager))]
    public partial class GestureManager : Singleton<GestureManager>
    {
        /// <summary>
        /// To select even when a hologram is not being gazed at,
        /// set the override focused object.
        /// If its null, then the gazed at object will be selected.
        /// </summary>
        public GameObject OverrideFocusedObject
        {
            get; set;
        }
        /// <summary>
        /// Gets the currently focused object, or null if none.
        /// </summary>
        public GameObject FocusedObject
        {
            get { return focusedObject; }
        }

        private UnityEngine.VR.WSA.Input.GestureRecognizer recognizer;
        private GameObject focusedObject;
        private Tool activeTool;

        public GameObject manipulationTarget { get; private set; }
        private Vector3 manipulationStartPos;

        void Awake()
        {
            recognizer = new UnityEngine.VR.WSA.Input.GestureRecognizer();
            recognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate | GestureSettings.Tap);
            recognizer.TappedEvent += GestureRecognizer_TappedEvent;
            recognizer.ManipulationStartedEvent += GestureRecognizer_ManipulationStartedEvent;
            recognizer.ManipulationUpdatedEvent += GestureRecognizer_ManipulationUpdatedEvent;
            recognizer.ManipulationCompletedEvent += GestureRecognizer_ManipulationCompletedEvent;
            recognizer.ManipulationCanceledEvent += GestureRecognizer_ManipulationCanceledEvent;
        }

        void Start()
        {
            ToolManager.Instance.ToolChanged += OnToolChanged;
            recognizer.StartCapturingGestures();
        }

        private void OnToolChanged(object sender, ToolManager.ToolChangedEventArgs e)
        {
            if (recognizer != null)
            {
                recognizer.CancelGestures();
            }
            activeTool = e.newTool;
        }

        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            if (activeTool != null)
            {
                activeTool.gameObject.SendMessage("OnSelect");
            }
            else
            {
                focusedObject.SendMessage("OnSelect");
            }
            Debug.Log("Tapped.");
        }

        private void GestureRecognizer_ManipulationStartedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            manipulationTarget = focusedObject;
            HandsManager.Instance.Hand.properties.location.TryGetPosition(out manipulationStartPos);
            Vector3 v = manipulationStartPos + cumulativeDelta;
            if (activeTool != null)
            {
                manipulationTarget = activeTool.gameObject;
            }
            if (manipulationTarget != null)
            {
                manipulationTarget.SendMessage("PerformManipulationStart", v);
            }
            Debug.Log("Manipulation started.");
        }

        private void GestureRecognizer_ManipulationUpdatedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            if (manipulationTarget != null)
            {
                Vector3 v = manipulationStartPos + cumulativeDelta;
                manipulationTarget.SendMessage("PerformManipulationUpdate", v);
            }
        }

        private void GestureRecognizer_ManipulationCompletedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            if (manipulationTarget != null)
            {
                manipulationTarget.SendMessage("PerformManipulationCompleted");
                Debug.Log("Manipulation completed.");
            }
            manipulationTarget = null;
        }

        private void GestureRecognizer_ManipulationCanceledEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            if (manipulationTarget != null)
            {
                manipulationTarget.SendMessage("PerformManipulationCanceled");
                Debug.Log("Manipulation canceled.");
            }
            manipulationTarget = null;
        }

        void LateUpdate()
        {
            GameObject oldFocusedObject = focusedObject;

            if (GazeManager.Instance.Hit &&
                OverrideFocusedObject == null &&
                GazeManager.Instance.HitInfo.collider != null)
            {
                // If gaze hits a hologram, set the focused object to that game object.
                // Also if the caller has not decided to override the focused object.
                focusedObject = GazeManager.Instance.HitInfo.collider.gameObject;
            }
            else
            {
                // If our gaze doesn't hit a hologram, set the focused object to null or override focused object.
                focusedObject = OverrideFocusedObject;
            }

            if (focusedObject != oldFocusedObject)
            {
                // If the currently focused object doesn't match the old focused object, cancel the current gesture.
                // Start looking for new gestures.  This is to prevent applying gestures from one hologram to another.
                if (manipulationTarget == null)
                {
                    recognizer.CancelGestures();
                    recognizer.StartCapturingGestures();
                }
            }
        }

        void OnDestroy()
        {
            recognizer.StopCapturingGestures();

            recognizer.TappedEvent -= GestureRecognizer_TappedEvent;
            recognizer.ManipulationStartedEvent -= GestureRecognizer_ManipulationStartedEvent;
            recognizer.ManipulationUpdatedEvent -= GestureRecognizer_ManipulationUpdatedEvent;
            recognizer.ManipulationCompletedEvent -= GestureRecognizer_ManipulationCompletedEvent;
            recognizer.ManipulationCanceledEvent -= GestureRecognizer_ManipulationCanceledEvent;
        }
    }
}