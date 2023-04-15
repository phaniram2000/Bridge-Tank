using UnityEngine;

public sealed class MovementState : InputStateBase
{
	private static float _motorTorque, _brakeTorque, _maxRpm;
	private static AnimationCurve _curve;

	private static bool _useMaxRpm;

	public MovementState()
	{
		if (!Player)
			InputHandler.AssignNewState(null);
		_motorTorque = Player.tank.motorTorque;
		_brakeTorque = Player.tank.brakeTorque;
		_curve = Player.tank.curve;
	}

	public override void OnEnter()
	{
		IsPersistent = true;
		
		foreach (var collider in Player.tank.wheelColliders)
			collider.brakeTorque = 0f;
		
		_maxRpm = Player.tank.maxRpm;
		_useMaxRpm = _maxRpm > 0f;
	}

	public override void FixedExecute()
	{
		HandleMotor();
		UpdateWheels();
	}

	public override void OnExit()
	{
		foreach (var collider in Player.tank.wheelColliders)
		{
			collider.motorTorque = 0f;
			collider.brakeTorque = _brakeTorque;
		}
	}

	private void HandleMotor()
	{
		for (var i = 0; i < Player.tank.wheelColliders.Length - 1; i += 2)
		{
			if (_useMaxRpm)
				if(Player.tank.wheelColliders[i].rpm > _maxRpm)
				{
					Player.tank.wheelColliders[i].motorTorque = Player.tank.wheelColliders[i + 1].motorTorque = 0f;
					return;
				}
			
			var curveVal = _curve.Evaluate(Mathf.InverseLerp(0, 450f, Player.tank.wheelColliders[i].rpm));
			var torque = Mathf.Lerp(0f, _motorTorque, curveVal);
			
			Player.tank.wheelColliders[i].motorTorque = Player.tank.wheelColliders[i + 1].motorTorque =
				Player.tank.wheelColliders[i].isGrounded ? torque : 0f;
		}

		DragChange();
	}

	private static void UpdateWheels()
	{
		UpdateSingleWheel(Player.tank.wheelColliders[0], Player.tank.wheels[0]);
		UpdateSingleWheel(Player.tank.wheelColliders[1], Player.tank.wheels[1]);
		UpdateSingleWheel(Player.tank.wheelColliders[6], Player.tank.wheels[2]);
		UpdateSingleWheel(Player.tank.wheelColliders[7], Player.tank.wheels[3]);
	}
	
	private static void DragChange()
	{
		rb.angularDrag = IsGrounded() ? Player.tank.initialAngularDrag : 0f;
		rb.drag = IsGrounded() ? Player.tank.initialDrag : 1f;
	}

	private static bool IsGrounded()
	{
		bool onGround = false;
		foreach (var wheel in Player.tank.wheelColliders)
		{
			if (wheel.isGrounded)
				onGround = true;
		}

		return onGround;
	}

	private static void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
	{
		wheelCollider.GetWorldPose(out var pos, out var rot);
		//this multiplication is so that you don't need to convert the axes of the wheel mesh in a DCC Software or use a parent transform
		wheelTransform.rotation = rot * Quaternion.Euler(Player.tank.initialRotation);
	}
}