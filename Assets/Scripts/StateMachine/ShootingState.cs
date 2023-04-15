using UnityEngine;

public class ShootingState : InputStateBase
{
	private static Transform _cannon;
	private static float _rotateSpeed, _clampAngleTo, _clampAngleFrom;

	public ShootingState()
	{
		_rotateSpeed = Player.rotateSpeed;
		_clampAngleTo = Player.clampAngleTo;
		_clampAngleFrom = Player.clampAngleFrom;
		_cannon = Player.cannon;

		IsPersistent = true;
	}

	public override void Execute()
	{
		if(!_cannon)
		{
			InputHandler.AssignNewState(null);
			return;
		}
		
		var inputY = InputExtensions.GetInputDelta().y;

		var localEulerAngles = _cannon.localEulerAngles;

		var angle = Convert(localEulerAngles.z);
		var newAngle = -inputY * _rotateSpeed * Time.deltaTime;
		
		if(angle + newAngle < _clampAngleFrom) return;
		if(angle + newAngle > _clampAngleTo) return;
		
		_cannon.Rotate(Vector3.forward * newAngle, Space.Self);
	}

	public override void OnExit()
	{
		Player.RecenterCannon();
	}

	private static float Convert(float angle)
	{
		if (angle > 180f)
			return  angle - 360f;

		return angle;
	}
}