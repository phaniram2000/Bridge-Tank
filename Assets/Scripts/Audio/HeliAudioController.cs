using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HeliAudioController : MonoBehaviour
{
	[SerializeField] private bool enableEngine;
	[SerializeField] private float idleVolume, combatVolume, minPitch, maxPitch, minVol, maxVol;
	
	private AudioSource _audioSource;
	private float _initYPos;
	private bool _isIdle = true, _isDead;

	private void OnEnable()
	{
		GameEvents.Singleton.driveEnd += OnDriveEnd;
	}

	private void OnDisable()
	{
		GameEvents.Singleton.driveEnd -= OnDriveEnd;
	}
	
	private void Start()
	{
		_audioSource = GetComponent<AudioSource>();
		//playing in low volume since we are not using 3d audio
		//volume is increased to normal at Drive end

		_initYPos = transform.position.y;
		_audioSource.volume = idleVolume;
	}

	private void Update()
	{
		if(_isIdle) return;
		if(_isDead) return;
		
		var diff = transform.position.y - _initYPos;

		_audioSource.volume = Mathf.Lerp(minPitch, maxPitch, Mathf.InverseLerp(0f, 1f, diff)); 
	}

	private void OnDriveEnd()
	{
		if (!enableEngine) return;

		_audioSource.volume = combatVolume;
		_isIdle = false;
	}

	private void OnLevelEnd()
	{
		if(!enableEngine) return;
		
		//dotween down the pitch slowly
	}


	public void OnHeliDeath()
	{
		_isDead = true;
		//volumes dotweens down
		DOTween.To(() => _audioSource.volume, value => _audioSource.volume = value,
			0, 5f);


		DOTween.To(() => _audioSource.pitch, value => _audioSource.pitch = value, _audioSource.pitch / 2f, 1f);


		//pitch dotweens a notch up quickly
	}
}
