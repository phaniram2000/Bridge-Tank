using UnityEngine;

public class FightAimTutorialController : MonoBehaviour
{
	//if input delta abs > 1f
	[SerializeField] private GameObject[] holders;
	[SerializeField] private float doneYDelta;

	private float _currentCumulativeYDelta;
	private bool _shouldCheck;

	private void OnEnable()
	{
		GameEvents.Singleton.driveEnd += OnDriveEnd;
		
		GameEvents.Singleton.levelEnd += OnLevelEnd;
	}

	private void OnDisable()
	{
		GameEvents.Singleton.driveEnd -= OnDriveEnd;
		
		GameEvents.Singleton.levelEnd -= OnLevelEnd;
	}

	private void Start()
	{
		ToggleAnimations(false);
	}

	private void Update()
	{
		if(!_shouldCheck) return;
		
		if(InputExtensions.GetFingerUp())
		{
			_currentCumulativeYDelta = 0f;
			return;
		}

		if (!InputExtensions.GetFingerHeld()) return;
		
		_currentCumulativeYDelta += Mathf.Abs(InputExtensions.GetInputDelta().y);
		
		if (_currentCumulativeYDelta < doneYDelta) return;
		
		ToggleAnimations(false);
	}

	private void ToggleAnimations(bool status)
	{
		foreach (var held in holders)
			held.SetActive(status);

		_shouldCheck = status;
	}

	private void OnDriveEnd()
	{
		ToggleAnimations(true);
	}
	
	private void OnLevelEnd(Faction loser)
	{
		gameObject.SetActive(false);
	}
}