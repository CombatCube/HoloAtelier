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
        Highlight,
        FreeDraw
    }

    public event EventHandler<ModeChangedEventArgs> ModeChanged;

    public Mode ActiveMode { get; private set; }

    void Awake ()
    {
    }

    // Use this for initialization
    void Start ()
    {
        ActiveMode = Mode.Highlight;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void LateUpdate()
    {

    }

    public void SetActiveMode(Mode mode)
    {
        ActiveMode = mode;
        EventHandler<ModeChangedEventArgs> modeChangedEvent = ModeChanged;
        ModeChangedEventArgs mcea = new ModeChangedEventArgs();
        mcea.newMode = mode;
        modeChangedEvent(this, mcea);
    }

    //public void OnLayers()
    //{
    //    gameObject.GetComponent<LayersPanel>().SetActive(true);
    //}

    public void OnHighlight()
    {
        SetActiveMode(Mode.Highlight);
    }

    public void OnFreeDraw()
    {
        SetActiveMode(Mode.FreeDraw);
    }

}
