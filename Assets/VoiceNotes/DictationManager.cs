using HoloToolkit.Unity;
using UnityEngine;

[RequireComponent(typeof(KeywordManager))]
public class DictationManager : Singleton<DictationManager>
{
    [Tooltip("Drag the Communicator prefab asset.")]
    public GameObject CommunicatorPrefab;
    private GameObject communicatorGameObject;

    public AudioClip DismissSound;

    public bool CommunicatorOpen { get; private set; }

    void Awake()
    {
        CommunicatorOpen = false;
    }

    public void OpenCommunicator()
    {
        CommunicatorOpen = true;

        communicatorGameObject = Instantiate(CommunicatorPrefab);

        communicatorGameObject.transform.position = transform.position;
        communicatorGameObject.transform.Translate(0.4f, 0.0f, 0.0f, Camera.main.transform);
    }

    public void CloseCommunicator()
    {
        CommunicatorOpen = false;

        GameObject soundPlayer = new GameObject("MessageSentSound");
        AudioSource soundSource = soundPlayer.AddComponent<AudioSource>();
        soundSource.clip = DismissSound;
        soundSource.Play();

        Destroy(communicatorGameObject);
        Destroy(soundPlayer, DismissSound.length);
    }

    void OnSelect()
    {
        OpenCommunicator();
    }
}