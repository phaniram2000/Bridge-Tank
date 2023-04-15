using UnityEngine;

public class InputStateBase
{
	public bool IsPersistent;
	protected static UnitController Player;
	protected static Camera Cam;
	protected static Rigidbody rb;

	protected InputStateBase() {}
	public InputStateBase(UnitController player, Camera cam, Rigidbody rigidbody)
	{
		Player = player;
		Cam = cam;
		rb = rigidbody;
	}

	public virtual void OnEnter() {}
	public virtual void Execute() {}

	public virtual void FixedExecute() {}
	
	public virtual void OnExit() {}

	public static void print(object message)
	{
		Debug.Log(message);
	}
}

public sealed class EndState : InputStateBase
{
	public override void OnEnter()
	{
		IsPersistent = true;
	}
}