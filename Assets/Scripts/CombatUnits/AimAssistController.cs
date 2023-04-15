using UnityEngine;

public class AimAssistController : MonoBehaviour
{
	public static bool IsEnabled;

	[SerializeField] private float scrollSpeed = 0.01f;

	private MeshRenderer _renderer;
	private Material _material;
	private bool _isWaiting = true;

	private void OnEnable()
	{
		GameEvents.Singleton.driveEnd += OnDriveEnd;
		GameEvents.Singleton.levelEnd += OnLevelEnd;
	}
	
	private void OnDisable()
	{
		GameEvents.Singleton.driveEnd -= OnDriveEnd;
		GameEvents.Singleton.levelEnd -= OnLevelEnd;
		
		_material.mainTextureOffset = Vector2.zero;
	}

	private void Start()
	{
		_renderer = GetComponent<MeshRenderer>();
		_material = _renderer.sharedMaterial;
		_renderer.enabled = false;
	}

	private void Update()
	{
		if(!IsEnabled) return;
		if(_isWaiting) return;
		
		_material.mainTextureOffset += Vector2.left * scrollSpeed;
	}

	private void OnDriveEnd()
	{
		if (!IsEnabled) return;
		
		_renderer.enabled = true;
		_isWaiting = false;
	}

	private void OnLevelEnd(Faction loser)
	{
		_renderer.enabled = IsEnabled = false;
	}
}
