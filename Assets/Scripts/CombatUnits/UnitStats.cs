using UnityEngine;

public enum Faction
{
	Player,
	Enemy
}

public enum UnitType
{
	Tank,
	Heli
}

public class UnitStats : MonoBehaviour
{
	public Faction myFaction;
	[HideInInspector] public UnitType myType;
	public float currentHealth, maxHealth, healthReductionByMissile;

	public bool isDead;
	
	private void Start()
	{
		currentHealth = maxHealth;
	}
	
	/// <summary>
	/// Returns if the player has died after this health Reduction or false
	/// </summary>
	/// <returns></returns>
	public bool ReduceHealth()
	{
		currentHealth -= healthReductionByMissile;
		
		return currentHealth < 0;
	}
}