﻿using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Sharing;
using System.Collections;
using System.Collections.Generic;
using System;

public class DrawCanvas : MonoBehaviour {

    public Material DrawMaterial;

    private GameObject lastLineObject;
    private Vector3[] lastPoints;

    public float DrawThreshold;

    public void StartLine(Vector3 position)
    {
        lastLineObject = new GameObject();
        lastLineObject.transform.SetParent(transform, false);
        LineRenderer line = lastLineObject.AddComponent<LineRenderer>();
        line.material = DrawMaterial;
        line.useWorldSpace = false;
        line.SetWidth(DrawThreshold, DrawThreshold);
        line.SetVertexCount(1);
        Vector3[] points = new Vector3[1];
        points[0] = lastLineObject.transform.InverseTransformPoint(position);
        line.SetPositions(points);
        lastPoints = points;
    }

    public void UpdateLine(Vector3 position)
    {
        LineRenderer line = lastLineObject.GetComponent<LineRenderer>();
        int nextPointIdx = lastPoints.Length;
        if ((lastPoints[nextPointIdx - 1] - lastLineObject.transform.InverseTransformPoint(position)).magnitude > DrawThreshold)
        {
            Vector3[] points = new Vector3[lastPoints.Length + 1];
            lastPoints.CopyTo(points, 0);
            points[nextPointIdx] = lastLineObject.transform.InverseTransformPoint(position);
            line.SetVertexCount(lastPoints.Length + 1);
            line.SetPositions(points);
            lastPoints = points;
        }
    }

    public void DrawLine(Vector3[] points)
    {
        GameObject lineObject = new GameObject();
        lineObject.transform.SetParent(transform, false);
        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.material = DrawMaterial;
        line.useWorldSpace = false;
        line.SetWidth(DrawThreshold, DrawThreshold);
        line.SetVertexCount(points.Length);
        line.SetPositions(points);
    }

    public void ClearCanvas()
    {
        if (GestureManager.Instance.manipulationTarget != null)
        {
            return;
        }
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SendStroke()
    {
        Note note = transform.GetComponentInParent<Note>();
        note.SendStroke(lastPoints);
    }
}
