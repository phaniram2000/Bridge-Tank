using UnityEngine;
using UnityEngine.UI;

public class HealthCanvasController : MonoBehaviour
{
	public Image healthBar;
	
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
		TurnCanvasActiveState(false);
	}

	public void UpdateHealth(int newHealth, float maxHealth)
	{
		healthBar.fillAmount = newHealth / maxHealth;

		if (healthBar.fillAmount >= 1f)
			healthBar.transform.parent.gameObject.SetActive(false);
		else
			healthBar.transform.parent.gameObject.SetActive(true);
	}
	
	private void OnDriveEnd()
	{
		TurnCanvasActiveState(true);
	}

	private void OnLevelEnd(Faction loser)
	{
		TurnCanvasActiveState(false);
	}
	
	private void TurnCanvasActiveState(bool state)
	{
		healthBar.transform.parent.parent.gameObject.SetActive(state);
	}
}
