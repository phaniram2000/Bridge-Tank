using DG.Tweening;
using UnityEngine;

public class MissileController : MonoBehaviour
{
	[SerializeField] private GameObject explosionFx;
	[SerializeField] private AudioSource audioSource;

	[SerializeField] private float pitchVariance = 0.2f;
	
	private void OnEnable()
	{
		GameEvents.Singleton.levelEnd += OnLevelEnd;
	}
	
	private void OnDisable()
	{
		GameEvents.Singleton.levelEnd -= OnLevelEnd;
	}

	private void Start()
	{
		audioSource.pitch += Random.Range(-pitchVariance, pitchVariance);
		audioSource.Play();
	}

	private void OnCollisionEnter(Collision other)
	{
		explosionFx.SetActive(true);
		explosionFx.transform.parent = null;
		Destroy(explosionFx, 2f);
		Destroy(gameObject);
	}

	private void OnLevelEnd(Faction loser)
	{
		transform.DOScale(Vector3.one * 0.1f, 1.98f);
		Destroy(gameObject, 2f);
	}
}
