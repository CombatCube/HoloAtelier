// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Broadcasts the head transform of the local user to other users in the session,
/// and adds and updates the head transforms of remote users.
/// Head transforms are sent and received in the local coordinate space of the GameObject
/// this component is on.
/// </summary>
public class RemoteHeadManager : Singleton<RemoteHeadManager>
{
    public GameObject remoteHeadObject;
    public class RemoteHeadInfo
    {
        public long UserID;
        public GameObject HeadObject;
    }

    public GameObject activeHead = null;
    public GameObject RemotePerspectiveTool;
    /// <summary>
    /// Keep a list of the remote heads, indexed by XTools userID
    /// </summary>
    public Dictionary<long, RemoteHeadInfo> remoteHeads = new Dictionary<long, RemoteHeadInfo>();
    bool cameraReset = true;

    void Start()
    {
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.HeadTransform] = this.UpdateHeadTransform;

        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
        SharingSessionTracker.Instance.SessionLeft += Instance_SessionLeft;
    }

    void Update()
    {
        // Grab the current head transform and broadcast it to all the other users in the session
        Transform headTransform = Camera.main.transform;

        // Transform the head position and rotation from world space into local space
        Vector3 headPosition = this.transform.InverseTransformPoint(headTransform.position);
        Quaternion headRotation = Quaternion.Inverse(this.transform.rotation) * headTransform.rotation;

        CustomMessages.Instance.SendHeadTransform(headPosition, headRotation);

        if (activeHead != null)
        {
            cameraReset = false;
            Camera.main.gameObject.transform.SetParent(activeHead.transform, true);
            Camera.main.gameObject.transform.localPosition = Vector3.Lerp(Camera.main.gameObject.transform.localPosition, Vector3.zero, 0.02f);
        }
        else
        {
            ResetCamera();
        }
    }

    private void ResetCamera()
    {
        if (!cameraReset)
        {
            Camera.main.gameObject.transform.SetParent(null, false);
            Camera.main.gameObject.transform.position = UnityEngine.VR.InputTracking.GetLocalPosition(UnityEngine.VR.VRNode.Head);
            cameraReset = true;
        }
    }

    /// <summary>
    /// Called when a new user is leaving.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Instance_SessionLeft(object sender, SharingSessionTracker.SessionLeftEventArgs e)
    {
        RemoveRemoteHead(this.remoteHeads[e.exitingUserId].HeadObject);
        this.remoteHeads.Remove(e.exitingUserId);
    }

    /// <summary>
    /// Called when a user is joining.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
        GetRemoteHeadInfo(e.joiningUser.GetID());
    }

    /// <summary>
    /// Gets the data structure for the remote users' head position.
    /// </summary>
    /// <param name="userID"></param>
    /// <returns></returns>
    public RemoteHeadInfo GetRemoteHeadInfo(long userID)
    {
        RemoteHeadInfo headInfo;

        // Get the head info if its already in the list, otherwise add it
        if (!this.remoteHeads.TryGetValue(userID, out headInfo))
        {
            headInfo = new RemoteHeadInfo();
            headInfo.UserID = userID;
            headInfo.HeadObject = CreateRemoteHead();

            this.remoteHeads.Add(userID, headInfo);
        }

        return headInfo;
    }

    /// <summary>
    /// Called when a remote user sends a head transform.
    /// </summary>
    /// <param name="msg"></param>
    void UpdateHeadTransform(NetworkInMessage msg)
    {
        // Parse the message
        long userID = msg.ReadInt64();

        Vector3 headPos = CustomMessages.Instance.ReadVector3(msg);

        Quaternion headRot = CustomMessages.Instance.ReadQuaternion(msg);

        RemoteHeadInfo headInfo = GetRemoteHeadInfo(userID);
        headInfo.HeadObject.transform.localPosition = headPos;
        headInfo.HeadObject.transform.localRotation = headRot;
    }

    /// <summary>
    /// Creates a new game object to represent the user's head.
    /// </summary>
    /// <returns></returns>
    GameObject CreateRemoteHead()
    {
        GameObject newHeadObj = Instantiate(remoteHeadObject);
        newHeadObj.transform.parent = this.gameObject.transform;
        return newHeadObj;
    }

    /// <summary>
    /// When a user has left the session this will cleanup their
    /// head data.
    /// </summary>
    /// <param name="remoteHeadObject"></param>
	void RemoveRemoteHead(GameObject remoteHeadObject)
    {
        DestroyImmediate(remoteHeadObject);
    }

    public void SetActiveHead(GameObject go)
    {
        activeHead = go;
        if (go != null)
        {
            RemotePerspectiveTool.GetComponent<RemotePerspective>().Activate();
        }
    }
}