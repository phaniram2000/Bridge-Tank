using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	[SerializeField] private float xOffset = 10f, yOffset = 2f, lerpSpeed = 2f;

	private bool _isEnabled = true;
	private Transform _player;

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
		_player = GameObject.FindGameObjectWithTag("Player").transform;

		xOffset = transform.position.x - _player.position.x;
		yOffset = transform.position.y - _player.position.y;
	}

	private void FixedUpdate()
	{
		if(!_isEnabled) return;
		
		var position = transform.position;
		var newPos = new Vector3(_player.position.x + xOffset, _player.position.y + yOffset, position.z);
		
		transform.position = Vector3.Slerp(position, newPos, lerpSpeed * Time.deltaTime);
	}
	
	private void OnDriveEnd()
	{
		_isEnabled = false;
	}
}
