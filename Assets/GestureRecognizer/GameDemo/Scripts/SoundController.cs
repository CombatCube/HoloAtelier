using UnityEngine;
using System.Collections;

public enum SoundAction { Play, Pause, Stop }

/// <summary>
/// Controls the music
/// </summary>
public class SoundController : MonoBehaviour
{
	/// <summary>
	/// Music clip
	/// </summary>
	public AudioClip music;

	/// <summary>
	/// Audio source of the music
	/// </summary>
	private AudioSource musicAS;


	void Awake()
	{
		GameObject go = new GameObject();
		go.name = "Music";
		go.transform.parent = transform;
		musicAS = go.AddComponent<AudioSource>();
		musicAS.volume = 0.8f;
		musicAS.clip = music;
		musicAS.loop = true;
		musicAS.playOnAwake = false;
	}


	void Start()
	{
		DoMusic(SoundAction.Play);
	}


	public void DoMusic(SoundAction soundAction)
	{

		switch (soundAction)
		{
			case SoundAction.Play: musicAS.Play(); break;
			case SoundAction.Pause: musicAS.Pause(); break;
			case SoundAction.Stop: musicAS.Stop(); break;
		}

	}

}