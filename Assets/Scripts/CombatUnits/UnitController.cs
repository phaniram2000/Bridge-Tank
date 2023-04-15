using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitController : MonoBehaviour
{
	public static UnitController Player;
	
	[Header("Aiming")] public Transform cannon;
	public float clampAngleFrom, clampAngleTo, rotateSpeed;
	[SerializeField] private ParticleSystem muzzleFlash;
	[SerializeField] private Transform muzzlePoint;
	[Tooltip("Only for Player"), SerializeField] private bool isAimAssistEnabled;
	
	[Header("Missile Launching"), SerializeField] private Transform missileSpawn;
	[SerializeField] private GameObject missilePrefab;
	[SerializeField] private float missileForce, waitTimeBetweenShots = 2f;

	[Header("Taking Hits"), SerializeField] private GameObject hitFx;
	[SerializeField] private GameObject smokeVfx, explosionFx, winCam;
	[SerializeField] private float explosionForce;
	
	private Vector3 _initialRotation;
	private float _currentMissileForce, _initialSmokeScale;
	private bool _isShooting;
	
	private Coroutine _shootingCoroutine;

	private Animator _anim;
	[HideInInspector] public Rigidbody rb;
	private UnitStats _stats;
	private HealthCanvasController _health;

	[HideInInspector] public TankController tank;
	[HideInInspector] public  HelicopterController heli;
	
	private WaitForSeconds _myWaitTime;
	private Vector3 _normal;
	
	private static readonly int Fire = Animator.StringToHash("fire");
	private static readonly int IsHeli = Animator.StringToHash("isHeli");

	private void OnEnable()
	{
		GameEvents.Singleton.driveStart += OnDriveStart;
		GameEvents.Singleton.driveEnd += OnDriveEnd;
		GameEvents.Singleton.levelEnd += OnLevelEnd;
	}
	
	private void OnDisable()
	{
		GameEvents.Singleton.driveStart -= OnDriveStart;
		GameEvents.Singleton.driveEnd -= OnDriveEnd;
		GameEvents.Singleton.levelEnd -= OnLevelEnd;
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		_stats = GetComponent<UnitStats>();
		_health = GetComponent<HealthCanvasController>();
		_anim = GetComponent<Animator>();

		TryGetComponent(out tank);
		TryGetComponent(out heli);
		

		if(heli)
			_anim.SetBool(IsHeli, true);

		if (_stats.myFaction == Faction.Enemy) return;

		if (!Player) Player = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		_currentMissileForce = missileForce;
		
		_initialRotation = cannon.localEulerAngles;
		_initialSmokeScale = smokeVfx.transform.localScale.x;
		
		AimAssistController.IsEnabled = isAimAssistEnabled;

		_myWaitTime = new WaitForSeconds(waitTimeBetweenShots);

		//Time.timeScale = 1.25f;
		
		if(_stats.myFaction == Faction.Player) return;
		
		GameEvents.Singleton.InvokeEnemyBirth();
	}

	private void Update()
	{
		if(_stats.myFaction == Faction.Enemy) return;
		
		if (Input.GetKeyUp(KeyCode.Q)) Death();
	}

	private void OnCollisionEnter(Collision other)
	{
		if (!other.collider.CompareTag("Missile")) return;

		Destroy(Instantiate(hitFx, other.contacts[0].point, Quaternion.LookRotation(-other.contacts[0].normal)), 3f);
		
		if (_stats.ReduceHealth())
			Death();
		
		_health.UpdateHealth((int)_stats.currentHealth, _stats.maxHealth);
/*
		if (_stats.currentHealth < _stats.maxHealth / 2)
			smokeVfx.SetActive(true);*/

		if (_stats.currentHealth < _stats.maxHealth * 0.25f)
			smokeVfx.transform.DOScale(Vector3.one * _initialSmokeScale * 2f, 2f);
		
		if(_stats.myFaction == Faction.Enemy)
			Vibration.Vibrate(20);
	}

	private void Death()
	{
		if(_stats.isDead) return;
		
		_stats.isDead = true;
		if(_stats.myFaction == Faction.Enemy)
			GameEvents.Singleton.InvokeEnemyDeath();
		else
			GameEvents.Singleton.InvokeLevelEnd(Faction.Player);
		
		_anim.enabled = false;
		winCam.SetActive(false);
		explosionFx.SetActive(true);
		explosionFx.transform.parent = null;
		explosionFx.transform.localScale *= 2f;
		
		if(_stats.myType == UnitType.Tank)
			tank.TankDeath(explosionForce);
		else
			heli.HeliDeath(explosionForce);
		
		Vibration.Vibrate(30);
	}

	private void OnDriveStart()
	{
		rb.isKinematic = false;
	}
	
	private void OnDriveEnd()
	{
		_shootingCoroutine = StartCoroutine(StartShooting());
		_isShooting = true;
	}

	private void OnLevelEnd(Faction loser)
	{
		if(!_isShooting) return;
		
		StopCoroutine(_shootingCoroutine);
		_isShooting = false;

		_stats.healthReductionByMissile = 0f;
	}

	private IEnumerator StartShooting()
	{
		yield return new WaitForSeconds(Random.value);
		while (_isShooting)
		{
			if (_stats.myFaction == Faction.Enemy)
			{
				if(!RandomiseAngleAndPower())
					yield break;
			}
			yield return _myWaitTime;
			_anim.SetTrigger(Fire);
		}
	}

	private bool RandomiseAngleAndPower()
	{
		var newAngle = Random.Range(clampAngleFrom, clampAngleTo);
		
		cannon.DOLocalRotate(new Vector3(0f, cannon.localEulerAngles.y, newAngle), 1f);
		_currentMissileForce = Random.Range(missileForce - 5f, missileForce + 2.5f);
		return true;
	}

	public void ShootRocketFromAnimator()
	{
		var missile = Instantiate(missilePrefab, missileSpawn.position, missileSpawn.rotation);
		Destroy(missile, 4.5f);
		var missileRb = missile.GetComponent<Rigidbody>();

		Destroy(Instantiate(muzzleFlash, muzzlePoint.position, muzzlePoint.rotation), 1.5f);
		missileRb.AddForce(missile.transform.forward * _currentMissileForce, ForceMode.Impulse);
		missileRb.AddTorque(-missile.transform.right * _currentMissileForce / 3.14f, ForceMode.Impulse);
		
		Vibration.Vibrate(15);
	}

	public void RecenterCannon()
	{
		cannon.DOLocalRotate(_initialRotation, 4f);
	}
}
