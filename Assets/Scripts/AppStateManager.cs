using UnityEngine;
using HoloToolkit.Unity;

/// <summary>
/// Keeps track of the current state of the experience.
/// </summary>
public class AppStateManager : Singleton<AppStateManager>
{
    /// <summary>
    /// Enum to track progress through the experience.
    /// </summary>
    public enum AppState
    {
        Starting = 0,
        PickingAvatar,
        WaitingForAnchor,
        WaitingForStageTransform,
        Ready
    }
    
    /// <summary>
    /// Tracks the current state in the experience.
    /// </summary>
    public AppState CurrentAppState { get; set; }

    void Start()
    {
        UIManager.Instance.SetActiveMode(UIManager.Mode.Select);
        // We start in the 'picking avatar' mode.
        //CurrentAppState = AppState.PickingAvatar;
        CurrentAppState = AppState.WaitingForAnchor;
        // Spatial mapping should be disabled when we start up so as not
        // to distract from the avatar picking.
        SpatialMappingManager.Instance.StopObserver();
        SpatialMappingManager.Instance.gameObject.SetActive(false);

        // On device we start by showing the avatar picker.
        //PlayerAvatarStore.Instance.SpawnAvatarPicker();
    }

    public void ResetStage()
    {
        // If we fall back to waiting for anchor, everything needed to 
        // get us into setting the target transform state will be setup.
        //if (CurrentAppState != AppState.PickingAvatar)
        //{
            CurrentAppState = AppState.WaitingForAnchor;
        //}
    }

    void Update()
    {
        switch (CurrentAppState)
        {
            //case AppState.PickingAvatar:
            //    // Avatar picking is done when the avatar picker has been dismissed.
            //    if (PlayerAvatarStore.Instance.PickerActive == false)
            //    {
            //        CurrentAppState = AppState.WaitingForAnchor;
            //    }
            //    break;
            case AppState.WaitingForAnchor:
                // Once the anchor is established we need to run spatial mapping for a 
                // little while to build up some meshes.
                if (ImportExportAnchorManager.Instance.AnchorEstablished)
                {
                    CurrentAppState = AppState.WaitingForStageTransform;
                    GestureManager.Instance.OverrideFocusedObject = HologramPlacement.Instance.gameObject;

                    SpatialMappingManager.Instance.gameObject.SetActive(true);
                    SpatialMappingManager.Instance.DrawVisualMeshes = true;
                    SpatialMappingManager.Instance.StartObserver();
                }
                break;
            case AppState.WaitingForStageTransform:
                // Now if we have the stage transform we are ready to go.
                if (HologramPlacement.Instance.GotTransform)
                {
                    CurrentAppState = AppState.Ready;
                }
                break;
        }
    }
}