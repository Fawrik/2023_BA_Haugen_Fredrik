using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Instance;
	public GameObject soundSource;
	public AudioSource _musicSource;
	public AudioClip[] _clips;
	public AudioSource audioSource;

	private void Awake()
	{
		Instance = Instance ?? this;

		if (this.gameObject.GetComponent<AudioSource>() == null)
		{
			_musicSource = this.gameObject.AddComponent<AudioSource>();
			_musicSource.playOnAwake = false;
		}
		else
		{
			_musicSource = this.gameObject.GetComponent<AudioSource>();
		}
	}

	public void PlayClip(AudioClip clip)
	{
		audioSource.PlayOneShot(clip);
	}

	public void PlaySound(AudioClip sound)
	{
		GameObject instance = Instantiate(soundSource, this.gameObject.transform);

		instance.GetComponent<AudioSource>().clip = sound;

		float time = instance.GetComponent<AudioSource>().clip.length;

		Destroy(instance, time + .25f);

		instance.GetComponent<AudioSource>().Play();
	}
}
