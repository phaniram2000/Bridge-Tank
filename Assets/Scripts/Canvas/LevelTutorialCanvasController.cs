using UnityEngine;
using UnityEngine.UI;

public class LevelTutorialCanvasController : MonoBehaviour
{
	[SerializeField] private GameObject[] toDisable;
	
	private enum HelpType { LineDrawing, Clicking };
	
	[SerializeField] private HelpType myHelp;
	
	[SerializeField] private Image image;
	[SerializeField] private Animation anim;

	private bool _lineSubscribed;
	
	private void OnEnable()
	{
		if (myHelp == HelpType.Clicking)
			GameEvents.Singleton.driveStart += StopDriveButtonAnim;
		else
		{
			GameEvents.Singleton.addPointToPath += DrawPathStarted;
			_lineSubscribed = true;
		}

		GameEvents.Singleton.levelEnd += OnLevelEnd;
	}
	
	private void OnDisable()
	{
		if (myHelp == HelpType.Clicking)
			GameEvents.Singleton.driveStart -= StopDriveButtonAnim;
		else
		{
			GameEvents.Singleton.addPointToPath -= DrawPathStarted;
			_lineSubscribed = false;
		}
		GameEvents.Singleton.levelEnd -= OnLevelEnd;
	}

	private void Start()
	{
		image.enabled = false;
		Invoke(nameof(PlayClickDriveAnim), 1f);
	}

	private void PlayClickDriveAnim()
	{
		image.enabled = true;
		anim.Play();
	}

	private void StopDriveButtonAnim()
	{
		gameObject.SetActive(false);
		anim.Stop();
	}

	private void DrawPathStarted(Vector2 point)
	{
		if(!_lineSubscribed) return;
		
		GameEvents.Singleton.addPointToPath -= DrawPathStarted;
		_lineSubscribed = false;
		
		anim.Stop();

		foreach (var thing in toDisable)
		{
			thing.SetActive(false);
		}
	}

	private void OnLevelEnd(Faction loser)
	{
		gameObject.SetActive(false);
	}
}