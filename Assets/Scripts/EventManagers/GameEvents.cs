using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEvents : MonoBehaviour
{
	public static GameEvents Singleton;

	//Path Drawing
	public Action<Vector2> addPointToPath;
	public Action buttonMechanic, endPath;

	//Game Flow
	public Action driveStart, driveEnd;
	public Action<Faction> levelEnd;
	
	//Enemy life cycle
	public Action enemyBirth, enemyDeath;
	
	private void Awake()
	{
		if (!Singleton) Singleton = this;
		else Destroy(gameObject);
		//
		// if(GAScript.Instance)
		// 	GAScript.Instance.LevelStart(PlayerPrefs.GetInt("levelNo", 1).ToString());
		
		TinySauce.OnGameStarted(PlayerPrefs.GetInt("levelNo", 1).ToString());
	}

	public void InvokeEnemyBirth() => enemyBirth?.Invoke();
	
	public void InvokeAddPointToPath(Vector2 newPosition)
	{
		addPointToPath?.Invoke(newPosition);
	}

	public void InvokeEndPath()
	{
		endPath?.Invoke();
	}
	
	public void InvokeDriveStart()
	{
		driveStart?.Invoke();
	}

	public void InvokeDriveEnd()
	{
		driveEnd?.Invoke();
	}

	public void InvokeEnemyDeath()
	{
		enemyDeath?.Invoke();
	}
	
	public void InvokeLevelEnd(Faction loser)
	{
		levelEnd?.Invoke(loser);
		
		TinySauce.OnGameFinished(loser == Faction.Enemy, 0, PlayerPrefs.GetInt("levelNo", 1).ToString());
		
		//
		// if (!GAScript.Instance) return;
		//
		// if (loser == Faction.Player)
		// 	GAScript.Instance.LevelFail(PlayerPrefs.GetInt("levelNo", 1).ToString());
		// else
		// 	GAScript.Instance.LevelCompleted(PlayerPrefs.GetInt("levelNo", 1).ToString());
	}
	
	public void InvokeButtonMechanic()
	{
		buttonMechanic?.Invoke();
	}
}
