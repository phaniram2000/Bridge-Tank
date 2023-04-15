using UnityEngine;

public class LevelFlowController : MonoBehaviour
{
	public static LevelFlowController Singleton;
	
	[SerializeField] private float slowMotionTimeScale;

	public int noOfEnemiesInLevel, noOfEnemiesKilled;
	public bool isDrawingPersistent;
	
	private void OnEnable()
	{
		GameEvents.Singleton.enemyBirth += OnEnemyBirth;
		GameEvents.Singleton.enemyDeath += OnEnemyKilled;
	}

	private void OnDisable()
	{
		GameEvents.Singleton.enemyBirth -= OnEnemyBirth;
		GameEvents.Singleton.enemyDeath -= OnEnemyKilled;
	}

	private void Awake()
	{
		if(!Singleton) Singleton = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		Vibration.Init();
		slowMotionTimeScale *= Time.timeScale;
		
		if(!isDrawingPersistent) return;
		Invoke(nameof(MakeDrawingPersistent), .5f);
	}
	
	private void MakeDrawingPersistent()
	{
		InputHandler.Only.MakeDrawPersistent();
	}

	private void OnEnemyBirth() => noOfEnemiesInLevel++;

	private void OnEnemyKilled()
	{
		if (++noOfEnemiesKilled < noOfEnemiesInLevel) return;
		
		GameEvents.Singleton.InvokeLevelEnd(Faction.Enemy);
		AudioManager.Only.Play("Fatake");
	}

	public void EnterSlowMotion()
	{
		Time.timeScale = slowMotionTimeScale;
	}

	public void LeaveSlowMotion()
	{
		Time.timeScale = 1f;
	}
}
