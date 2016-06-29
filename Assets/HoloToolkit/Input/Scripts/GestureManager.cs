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
        public GameObject Canvas;
        /// <summary>
        /// Gets the currently focused object, or null if none.
        /// </summary>
        public GameObject FocusedObject
        {
            get { return focusedObject; }
        }

        private GestureRecognizer TapRecognizer;
        private GestureRecognizer ManipulationRecognizer;
        private GestureRecognizer ActiveRecognizer;
        private GameObject focusedObject;

        public bool IsManipulating { get; private set; }
        private Vector3 manipulationStartPos;
        private Vector3 cameraHandOffset;

        void Awake()
        {
            // Create a new GestureRecognizer. Sign up for tapped events.
            TapRecognizer = new GestureRecognizer();
            TapRecognizer.SetRecognizableGestures(GestureSettings.Tap);
            TapRecognizer.TappedEvent += GestureRecognizer_TappedEvent;

            ManipulationRecognizer = new GestureRecognizer();
            ManipulationRecognizer.SetRecognizableGestures(GestureSettings.ManipulationTranslate);
            ManipulationRecognizer.ManipulationStartedEvent += GestureRecognizer_ManipulationStartedEvent;
            ManipulationRecognizer.ManipulationUpdatedEvent += GestureRecognizer_ManipulationUpdatedEvent;
            ManipulationRecognizer.ManipulationCompletedEvent += GestureRecognizer_ManipulationCompletedEvent;
            ManipulationRecognizer.ManipulationCanceledEvent += GestureRecognizer_ManipulationCanceledEvent;
        }

        void Start()
        {
            Transition(TapRecognizer);
            UIManager.Instance.ModeChanged += OnModeChanged;
        }

        private void OnModeChanged(object sender, UIManager.ModeChangedEventArgs mcea)
        {
            switch (mcea.newMode)
            {
                case UIManager.Mode.Highlight:
                    Transition(TapRecognizer);
                    break;
                case UIManager.Mode.FreeDraw:
                    OverrideFocusedObject = Canvas;
                    Transition(ManipulationRecognizer);
                    break;
                default:
                    Transition(TapRecognizer);
                    break;
            }
        }

        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            if (focusedObject != null)
            {
                focusedObject.SendMessage("OnSelect");
            }
        }

        private void GestureRecognizer_ManipulationStartedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            IsManipulating = true;
            HandsManager.Instance.Hand.properties.location.TryGetPosition(out manipulationStartPos);
            focusedObject.SendMessage("PerformManipulationStart", manipulationStartPos + cumulativeDelta);
        }

        private void GestureRecognizer_ManipulationUpdatedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            if (IsManipulating)
            {
                Vector3 v = manipulationStartPos + cumulativeDelta;
                focusedObject.SendMessage("PerformManipulationUpdate", v);
            }
        }

        private void GestureRecognizer_ManipulationCompletedEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            IsManipulating = false;
            Canvas.SendMessage("PerformManipulationCompleted");
        }

        private void GestureRecognizer_ManipulationCanceledEvent(InteractionSourceKind source, Vector3 cumulativeDelta, Ray headRay)
        {
            //IsManipulating = false;
            //Canvas.SendMessage("PerformManipulationCanceled");
        }

        /// <summary>
        /// Transition to a new GestureRecognizer.
        /// </summary>
        /// <param name="newRecognizer">The GestureRecognizer to transition to.</param>
        public void Transition(GestureRecognizer newRecognizer)
        {
            if (newRecognizer == null)
            {
                return;
            }

            if (ActiveRecognizer != null)
            {
                if (ActiveRecognizer == newRecognizer)
                {
                    return;
                }

                ActiveRecognizer.CancelGestures();
                ActiveRecognizer.StopCapturingGestures();
            }
            newRecognizer.StartCapturingGestures();
            ActiveRecognizer = newRecognizer;
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

                // TODO: If looking at the UI panel, switch to the TapRecognizer.
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
                ActiveRecognizer.CancelGestures();
                ActiveRecognizer.StartCapturingGestures();
            }
        }

        void OnDestroy()
        {
            ActiveRecognizer.StopCapturingGestures();

            TapRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;

            ManipulationRecognizer.ManipulationStartedEvent -= GestureRecognizer_ManipulationStartedEvent;
            ManipulationRecognizer.ManipulationUpdatedEvent -= GestureRecognizer_ManipulationUpdatedEvent;
            ManipulationRecognizer.ManipulationCompletedEvent -= GestureRecognizer_ManipulationCompletedEvent;
            ManipulationRecognizer.ManipulationCanceledEvent -= GestureRecognizer_ManipulationCanceledEvent;
        }
    }
}