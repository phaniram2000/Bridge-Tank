using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TimepassTroopController : MonoBehaviour
{
	private static readonly List<GameObject> EnemyTroops = new List<GameObject>(); 
	private static readonly List<GameObject> PlayerTroops = new List<GameObject>();

	[SerializeField] private Transform winPos;
	[SerializeField] private Faction myFaction;
	[SerializeField] private bool isMounted;

	[SerializeField] private GameObject spawnFx;
	[SerializeField] private GameObject[] meshes;
	private static int _deathAnimInt;

	private UnitStats _stats;

	private static int DeathAnimInt { get => _deathAnimInt; set => _deathAnimInt = value % 5; }

	private Animator _anim;
	private static readonly int Death = Animator.StringToHash("death");
	private static readonly int DeathAnim = Animator.StringToHash("deathAnim");
	private static readonly int Win = Animator.StringToHash("win");
	private static readonly int WinSpeedMultiplier = Animator.StringToHash("winSpeedMultiplier");
	private static readonly int WinMirror = Animator.StringToHash("winMirror");

	private void OnEnable()
	{
		GameEvents.Singleton.driveEnd += OnCombatStart;
		GameEvents.Singleton.levelEnd += OnLevelEnd;
	}
	
	private void OnDisable()
	{
		GameEvents.Singleton.driveEnd -= OnCombatStart;
		GameEvents.Singleton.levelEnd -= OnLevelEnd;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		_stats = transform.root.GetComponent<UnitStats>();

		(myFaction == Faction.Enemy ? EnemyTroops : PlayerTroops).Add(gameObject);
		
		if(isMounted) return;
		foreach (var mesh in meshes)
			mesh.SetActive(false);
	}

	private void OnCombatStart()
	{
		spawnFx.SetActive(true);
		
		foreach (var mesh in meshes)
			mesh.SetActive(true);
	}

	private void OnLevelEnd(Faction loser)
	{
		if(isMounted)
		{
			if(myFaction == loser)
			{
				transform.parent = null;
				
				if(TryGetComponent(out CapsuleCollider coll))
				{
					coll.attachedRigidbody.isKinematic = false;
					coll.isTrigger = false;
					coll.direction = 2;
				}
			}
			else
			{
				if(_stats.isDead) return;
				DOTween.Sequence().AppendInterval(1).Append(
					transform.DOMove(winPos.position, 0.5f)).Append(
					transform.DORotateQuaternion(winPos.rotation, 0.5f));
			}
		}
		
		//regardless of mounting, every troop
		if(myFaction != loser)
		{
			_anim.SetFloat(WinSpeedMultiplier, Random.Range(0.85f, 1.15f));
			_anim.SetBool(WinMirror, Random.value > 0.5f);
			_anim.SetTrigger(Win);
		}
		else
			StartCoroutine(DeathLoop());
	}

	private IEnumerator DeathLoop()
	{
		var x = (myFaction == Faction.Enemy ? EnemyTroops : PlayerTroops).Count;
		
		while(x-- > 0)
		{
			yield return GameExtensions.GetWaiter(.25f);
			if (gameObject != (myFaction == Faction.Enemy ? EnemyTroops : PlayerTroops)[x]) continue;
			
			_anim.SetInteger(DeathAnim, DeathAnimInt++);
			DOTween.Sequence().AppendInterval(.25f).AppendCallback(() => _anim.SetTrigger(Death));
		}
	}
}