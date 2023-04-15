using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TankAudioController : MonoBehaviour
{
	[SerializeField] private WheelCollider anyRearWheel;
	
	[Header("Engine")] public bool enableEngine = true;
	[SerializeField] private AudioClip engineStart, engineStop;
	[SerializeField] private float recordedMaxRpm;
	[SerializeField] private float minPitch, maxPitch, minVolume, maxVolume;

	private AudioSource _audioSource;
	private float _currentVolume, _currentPitch;

	private void OnEnable()
	{
		GameEvents.Singleton.driveStart += OnDriveStart;
		GameEvents.Singleton.driveEnd += OnDriveEnd;
	}

	private void OnDisable()
	{
		GameEvents.Singleton.driveStart -= OnDriveStart;
		GameEvents.Singleton.driveEnd -= OnDriveEnd;
	}
	
	private void Start()
	{
		_audioSource = GetComponent<AudioSource>();
	}
	
	private void Update()
	{
		if (!enableEngine) return;

		if (!anyRearWheel.isGrounded)
		{
			_currentPitch = Mathf.Lerp(_currentPitch, minPitch, Time.deltaTime);
			_currentVolume = minVolume;
		}
		else
		{
			var rpm = anyRearWheel.rpm;

			var t = Mathf.InverseLerp(0, recordedMaxRpm, rpm);

			_currentPitch = Mathf.Lerp(minPitch, maxPitch, t);
			_currentVolume = Mathf.Lerp(minVolume, maxVolume, t);
		}
		
		_audioSource.volume = Mathf.Clamp(_currentVolume, minVolume, maxVolume);
		_audioSource.pitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);
	}
	
	private void OnDriveStart()
	{
		if (!enableEngine) return;
		_audioSource.PlayOneShot(engineStart, _currentVolume);
		_audioSource.Play();
	}
	
	private void OnDriveEnd()
	{
		if (!enableEngine) return;
		
		Invoke(nameof(WaitAndStopEngine), 1f);
	}

	private void WaitAndStopEngine()
	{
		_audioSource.Stop();
		_audioSource.PlayOneShot(engineStop, 1f);
		enableEngine = false;
	}
}
