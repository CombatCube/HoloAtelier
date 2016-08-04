using HoloToolkit.Unity;
using System.Collections;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.Windows.Speech;

/// <summary>
/// This keeps track of the various parts of the recording and text display process.
/// </summary>

[RequireComponent(typeof(AudioSource), typeof(MicrophoneManager))]
public class VoiceNote : Note
{

    [Tooltip("The sound to be played when the recording session starts.")]
    public AudioClip StartListeningSound;
    [Tooltip("The sound to be played when the recording session ends.")]
    public AudioClip StopListeningSound;

    private AudioSource dictationAudio;
    private AudioSource startAudio;
    private AudioSource stopAudio;
    private bool stopped;
    public enum Message
    {
        PressMic,
        PressStop,
        SendMessage
    };

    public MicrophoneManager microphoneManager;

    void Start()
    {
    }

    public void Record()
    {
        dictationAudio = gameObject.GetComponent<AudioSource>();
        microphoneManager = GetComponent<MicrophoneManager>();
        startAudio = gameObject.AddComponent<AudioSource>();
        stopAudio = gameObject.AddComponent<AudioSource>();
        startAudio.playOnAwake = false;
        startAudio.clip = StartListeningSound;
        stopAudio.playOnAwake = false;
        stopAudio.clip = StopListeningSound;
        // Turn the microphone on, which returns the recorded audio.
        dictationAudio.clip = microphoneManager.StartRecording();
        // Set proper UI state and play a sound.
        SetUI(true, Message.PressStop, startAudio);
    }

    public void RecordStop()
    {
        if (!stopped)
        {
            stopped = true;
            // Turn off the microphone.
            microphoneManager.StopRecording();
            microphoneManager.StartCoroutine("RestartSpeechSystem", NoteManager.Instance.KeywordManager);
            // Set proper UI state and play a sound.
            SetUI(false, Message.SendMessage, stopAudio);
        }
    }

    public void Play()
    {
        dictationAudio.Play();
    }

    public void PlayStop()
    {
        dictationAudio.Stop();
    }

    void ResetAfterTimeout()
    {
        // Set proper UI state and play a sound.
        SetUI(false, Message.PressMic, stopAudio);
    }

    private void SetUI(bool enabled, Message newMessage, AudioSource soundToPlay)
    {
        soundToPlay.Play();
    }

    new void OnSelect()
    {
        base.OnSelect();
        if (collapsed)
        {
            GetComponent<VoiceNote>().Play();
        }
        else
        {
            GetComponent<VoiceNote>().PlayStop();
        }
    }
}
