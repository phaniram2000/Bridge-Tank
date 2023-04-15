using UnityEngine;

public class MeshController : MonoBehaviour
{
	public bool shouldMakeIdle = true;
	[SerializeField] private float minTimeIdle = 1f;
	private float _elapsedIdle;
	private bool _isIdle, _isMeshDrawComplete;

	[Header("Path Drawing Audio")] public bool enableAudio;
	[SerializeField] private float recordedMaxTouchDelta;
	[SerializeField] private float minPitch, maxPitch, minVolume, maxVolume;
	
	//Private Audio Variables
	private AudioSource _audioSource;
	private float _currentVolume, _currentPitch;
	
	private Rigidbody _rb;
	
	private void OnEnable()
	{
		GameEvents.Singleton.endPath += OnEndPath;

		GameEvents.Singleton.driveEnd += DeletePath;
	}
    
	private void OnDisable()
	{
		GameEvents.Singleton.endPath -= OnEndPath;
		
		GameEvents.Singleton.driveEnd += DeletePath;
	}

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
		_audioSource = GetComponent<AudioSource>();
		
		_rb.useGravity = false;
		_rb.isKinematic = true;
	}

	private void Update()
	{
		if(!enableAudio) return;

		var delta = InputExtensions.GetInputDelta().magnitude;

		var t = Mathf.InverseLerp(0, recordedMaxTouchDelta, delta);

		_currentPitch = Mathf.Lerp(minPitch, maxPitch, t);
		_currentVolume = Mathf.Lerp(minVolume, maxVolume, t);
		
		_audioSource.volume = Mathf.Clamp(_currentVolume, minVolume, maxVolume);
		_audioSource.pitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);
	}

	private void FixedUpdate()
	{
		if(!shouldMakeIdle) return;
		
		if(_isIdle) return;
		if(!_isMeshDrawComplete) return;

		if (Mathf.Abs(_rb.velocity.sqrMagnitude) > 0)
		{
			_elapsedIdle = 0f;
			return;
		}

		_elapsedIdle += Time.fixedDeltaTime;
		if (_elapsedIdle < minTimeIdle) return;
		
		_isIdle = true;
		_rb.useGravity = false;
		_rb.isKinematic = true;
	}

	private void OnEndPath()
	{
		if(_isIdle) return;
		
		_rb.isKinematic = false;
		_rb.useGravity = true;
		_isMeshDrawComplete = true;

		enableAudio = false;
		_audioSource.Stop();
	}

	private void DeletePath()
	{
		Destroy(gameObject);
	}
}
