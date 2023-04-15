using UnityEngine;

public class EndTriggerController : MonoBehaviour
{
	private bool _driveEnded;

	private void OnEnable()
	{
		GameEvents.Singleton.levelEnd += OnLevelEnd;
	}

	private void OnDisable()
	{
		GameEvents.Singleton.levelEnd -= OnLevelEnd;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(_driveEnded) return;
		
		if(!other.CompareTag("Player")) return;

		_driveEnded = true;
		GameEvents.Singleton.InvokeDriveEnd();
	}
	
	private void PlayParticle()
	{
		foreach (var particle in GetComponentsInChildren<ParticleSystem>())
			particle.Play();
	}

	private void OnLevelEnd(Faction loser)
	{
		if (loser == Faction.Enemy) return;
		
		AudioManager.Only.Play("Fatake");

		Invoke(nameof(PlayParticle), .75f);
	}
}
