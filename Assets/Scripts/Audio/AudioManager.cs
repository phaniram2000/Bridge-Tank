using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	public static AudioManager Only;

	public Sound[] sounds;

    private void OnEnable() =>
        SceneManager.sceneLoaded += OnLevelLoaded;

    private void OnDisable() =>
		SceneManager.sceneLoaded -= OnLevelLoaded;

	private void Awake()
	{
		if (!Only)
		{
			Only = this;
			DontDestroyOnLoad(gameObject);
		}
		else Destroy(gameObject);

		foreach (var s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();

			s.source.clip = s.clip;
			s.source.loop = s.loop;
		}
	}

    private void OnLevelLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
		foreach (var source in GetComponents<AudioSource>())
			source.Stop();
    }

    public void Play(string sound, float pitch = -1f)
	{
		var s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + sound + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));

		if (pitch > 0f)
			s.source.pitch = pitch;
		else
			s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}

    public void Pause(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Pause();
    }
}