﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// HandsManager determines if the hand is currently detected or not.
    /// </summary>
    public partial class HandsManager : Singleton<HandsManager>
    {
        public InteractionSourceState Hand;
        public InteractionSourceState[] Hands;
        /// <summary>
        /// HandDetected tracks the hand detected state.
        /// Returns true if the list of tracked hands is not empty.
        /// </summary>
        public bool HandDetected
        {
            get { return trackedHands.Count > 0; }
        }

        public bool TwoHandsDetected
        {
            get { return trackedHands.Count > 1; }
        }

        private HashSet<uint> trackedHands = new HashSet<uint>();

        void Awake()
        {
            InteractionManager.SourceDetected += InteractionManager_SourceDetected;
            InteractionManager.SourceUpdated += InteractionManager_SourceUpdated;
            InteractionManager.SourceLost += InteractionManager_SourceLost;
        }

        private void InteractionManager_SourceDetected(InteractionSourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind != InteractionSourceKind.Hand)
            {
                return;
            }
            trackedHands.Add(state.source.id);
            if (trackedHands.Count > 1)
            {
                Hands = InteractionManager.GetCurrentReading();
            }
            else
            {
                Hand = state;
            }
        }

        private void InteractionManager_SourceUpdated(InteractionSourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.id != Hand.source.id)
            {
                return;
            }
            if (trackedHands.Count > 1)
            {
                Hands = InteractionManager.GetCurrentReading();
            }
            Hand = state;
        }

        private void InteractionManager_SourceLost(InteractionSourceState state)
        {
            // Check to see that the source is a hand.
            if (state.source.kind != InteractionSourceKind.Hand)
            {
                return;
            }

            if (trackedHands.Contains(state.source.id))
            {
                trackedHands.Remove(state.source.id);
            }

        }

        void OnDestroy()
        {
            InteractionManager.SourceDetected -= InteractionManager_SourceDetected;
            InteractionManager.SourceLost -= InteractionManager_SourceLost;
        }
    }
}