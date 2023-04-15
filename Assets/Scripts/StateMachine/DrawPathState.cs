using UnityEngine;

public sealed class DrawPathState : InputStateBase
{
	private readonly float _minDeltaBetweenPoints;

	private Vector2 _lastPos;

	public DrawPathState(float drawDelta)
	{
		_minDeltaBetweenPoints = drawDelta;
	}

	public override void OnEnter()
	{
		IsPersistent = false;
		_lastPos = default;
	}

	public override void Execute()
	{
		var ray = Cam.ScreenPointToRay(InputExtensions.GetInputPosition());
        
		if(!Physics.Raycast(ray, out var hit)) return;
		if(!hit.collider.CompareTag("MeshTrigger") && !hit.collider.CompareTag("GeneratedMesh"))
		{
			OnExit();
			return;
		}

		if (Vector2.Distance(_lastPos, hit.point) < _minDeltaBetweenPoints) return;

		_lastPos = hit.point;
		GameEvents.Singleton.InvokeAddPointToPath(hit.point);
	}

	public override void OnExit()
	{
		GameEvents.Singleton.InvokeEndPath();
	}
}
