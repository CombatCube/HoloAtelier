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

    public AudioSource dictationAudio;
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
            SendVoiceData();
            NoteManager.Instance.recording = false;
            Debug.Log("Clip recorded: frequency=" + dictationAudio.clip.frequency + ", channels=" + dictationAudio.clip.channels);
        }
    }

    void ResetAfterTimeout()
    {
        // Set proper UI state and play a sound.
        SetUI(false, Message.PressMic, stopAudio);
    }

    void SendVoiceData()
    {
        AudioClip clip = GetComponent<AudioSource>().clip;
        float[] data = new float[clip.samples * clip.channels];
        clip.GetData(data, 0);
        CustomMessages.Instance.SendVoiceNote(
            noteID,
            (byte)NoteType.Voice,
            transform.localPosition,
            transform.localRotation,
            microphoneManager.DictationDisplay.text,
            data
        );
    }

    private void SetUI(bool enabled, Message newMessage, AudioSource soundToPlay)
    {
        soundToPlay.Play();
    }

    public new void OnSelect()
    {
        base.OnSelect();
        if (!collapsed)
        {
            dictationAudio.Play();
        }
        else
        {
            dictationAudio.Stop();
        }
    }
}
