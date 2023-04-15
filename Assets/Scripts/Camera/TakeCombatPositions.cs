using DG.Tweening;
using UnityEngine;

public class TakeCombatPositions : MonoBehaviour
{
	[SerializeField] private bool masterSwitch = true;
	[SerializeField] private float xPositionPlayer = 20f, xPositionFightCam = 40f, duration = 1f;
	private Transform _player, _fightCam;

	private void OnEnable()
	{
		GameEvents.Singleton.driveEnd += TakePositions;
	}
	
	private void OnDisable()
	{
		GameEvents.Singleton.driveEnd -= TakePositions;
	}

	private void Start()
	{
		_player = GameObject.FindGameObjectWithTag("Player").transform.root;
		_fightCam = GameObject.FindGameObjectWithTag("FightCam").transform.root;
	}
	
	private void TakePositions()
	{
		if(!masterSwitch) return;
		_player.DOMoveX(xPositionPlayer, duration);
		_fightCam.DOMoveX(xPositionFightCam, duration);
	}
}
