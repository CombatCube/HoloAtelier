using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using System;

public partial class UIManager : Singleton<UIManager>
{
    public class ModeChangedEventArgs : EventArgs
    {
        public Mode newMode;
    }

    public enum Mode
    {
        GazeDraw,
        FreeDraw,
        Other
    }

    public event EventHandler<ModeChangedEventArgs> ModeChanged;

    public Mode ActiveMode { get; private set; }

    void Awake ()
    {
    }

    // Use this for initialization
    void Start ()
    {
        ActiveMode = Mode.Other;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {
        if (ActiveMode == Mode.Other)
        {
            SetActiveMode(Mode.FreeDraw);
        }
    }

    public void SetActiveMode(Mode mode)
    {
        ActiveMode = mode;
        EventHandler<ModeChangedEventArgs> modeChangedEvent = ModeChanged;
        ModeChangedEventArgs mcea = new ModeChangedEventArgs();
        mcea.newMode = mode;
        modeChangedEvent(this, mcea);
    }

    public void OnFreeDraw()
    {
        SetActiveMode(Mode.FreeDraw);
    }

    public void OnGazeDraw()
    {
        SetActiveMode(Mode.GazeDraw);
    }
}
