// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using HoloToolkit.Unity;
using UnityEngine;

/// <summary>
/// CursorManager class takes Cursor GameObjects.
/// One that is on Holograms and another off Holograms.
/// 1. Shows the appropriate Cursor when a Hologram is hit.
/// 2. Places the appropriate Cursor at the hit position.
/// 3. Matches the Cursor normal to the hit surface.
/// </summary>
public partial class CursorManager : Singleton<CursorManager>
{   
    public GameObject CursorDraw;
    public GameObject CursorDot;
    public GameObject CursorRing;

    private GameObject ActiveCursor;

    [Tooltip("Distance, in meters, to offset the cursor from the collision point.")]
    public float DistanceFromCollision = 0.01f;
    private List<GameObject> Cursors;

    public bool cursorAtHand { get; private set; }

    void Awake()
    {
        Cursors = new List<GameObject>();
        CursorRing.SetActive(false);
        Cursors.Add(CursorDraw);
        // Hide the Cursors to begin with.
        foreach (GameObject c in Cursors)
        {
            c.SetActive(false);
        }
    }

    void Start()
    {
        UIManager.Instance.ModeChanged += OnModeChanged;
        ActiveCursor = CursorDot;
    }

    void LateUpdate()
    {
        if (GazeManager.Instance == null || ActiveCursor == null)
        {
            return;
        }

        if (HandsManager.Instance.HandDetected)
        {
            CursorDot.SetActive(false);
            ActiveCursor.SetActive(true);
        }
        else
        {
            ActiveCursor.SetActive(false);
            CursorDot.SetActive(true);
        }
        if (!cursorAtHand && GazeManager.Instance.Hit)
        {
            CursorDraw.GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
        else
        {
            CursorDraw.GetComponent<MeshRenderer>().material.color = Color.white;
        }
        if (!cursorAtHand)
        {
            // Place the cursor at the calculated position.
            this.gameObject.transform.position = GazeManager.Instance.Position + GazeManager.Instance.Normal * DistanceFromCollision;
            // Orient the cursor to match the surface being gazed at.
            gameObject.transform.up = GazeManager.Instance.Normal;
        }
        else
        {
            // Place the cursor at the user's hand, if detected.
            if (HandsManager.Instance.HandDetected)
            {
                Vector3 pos;
                HandsManager.Instance.Hand.properties.location.TryGetPosition(out pos);
                gameObject.transform.position = pos;
                Quaternion v = Quaternion.LookRotation(-Camera.main.transform.up, Camera.main.transform.forward);
                gameObject.transform.rotation = v;
            }
        }
    }

    void OnModeChanged(object sender, UIManager.ModeChangedEventArgs mcea)
    {
        CursorDot.SetActive(false);
        foreach (GameObject go in Cursors)
        {
            go.SetActive(false);
        }
        switch (mcea.newMode)
        {
            case UIManager.Mode.Highlight:
                ActiveCursor = CursorRing;
                cursorAtHand = true;
                break;
            case UIManager.Mode.FreeDraw:
                ActiveCursor = CursorDraw;
                cursorAtHand = true;
                break;
            default:
                break;
        }
    }
}