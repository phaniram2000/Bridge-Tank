using UnityEngine;

public class InputHandler : MonoBehaviour
{
	public static InputHandler Only;
	
	public float touchInputDivisor = 50f;
	
	[Header("Path Drawing"), SerializeField] 
	private float pathDrawDelta = 1f;
	[SerializeField] private GameObject meshPrefab;

	private Camera _cam;
	private int _meshCount;
	private bool _isPersistentDrawingLevel;
	
	private static InputStateBase _currentInputState, _secondaryState;
	private static MovementState _movementState;
	private static DrawPathState _drawPathState;
	private static ShootingState _shootingState;
	private static EndState _endState;

	private void OnEnable()
	{
		GameEvents.Singleton.driveEnd += OnDriveEnd;
		GameEvents.Singleton.levelEnd += OnLevelEnd;
	}
	
	private void OnDisable()
	{
		GameEvents.Singleton.driveEnd -= OnDriveEnd;
		GameEvents.Singleton.levelEnd -= OnLevelEnd;
	}

	private void Awake()
	{
		if (!Only) Only = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		_cam = Camera.main;
		
		InputExtensions.IsUsingTouch = Application.platform != RuntimePlatform.WindowsEditor &&
									   (Application.platform == RuntimePlatform.Android || 
										Application.platform == RuntimePlatform.IPhonePlayer);
		InputExtensions.TouchInputDivisor = touchInputDivisor;

		_ = new InputStateBase(UnitController.Player, _cam, UnitController.Player.rb);

		_drawPathState = new DrawPathState(pathDrawDelta);
		_movementState = new MovementState();
		_shootingState = new ShootingState();
		_endState = new EndState();
		
		_currentInputState = null;
		_secondaryState = null;
	}

	private void Update()
	{
		if(_currentInputState == null)
		{
			_currentInputState = HandleInput();
			_currentInputState?.OnEnter();
		}
		else if (InputExtensions.GetFingerUp() && !_currentInputState.IsPersistent)
			AssignNewState(null);
		
		_currentInputState?.Execute();
		
		HandleSecondaryState();
	}

	private void FixedUpdate()
	{
		_currentInputState?.FixedExecute();
	}

	private InputStateBase HandleInput()
	{
		if (!InputExtensions.GetFingerHeld()) return null;
		
		var ray = _cam.ScreenPointToRay(InputExtensions.GetInputPosition());

		//if raycast didn't hit anything
		if (!Physics.Raycast(ray, out var hit)) return null;

		if (!hit.collider.CompareTag("MeshTrigger")) return null;
		
		var newMesh = Instantiate(meshPrefab);
		newMesh.name = "Mesh " + ++_meshCount;
		return _drawPathState;
	}

	private void HandleSecondaryState()
	{
		if(!_isPersistentDrawingLevel) return;
		if (_currentInputState != _movementState) return;

		if (_secondaryState == null)
		{
			_secondaryState = HandleInput();
			_secondaryState?.OnEnter();
			if(_secondaryState != null)
			{
				LevelFlowController.Singleton.EnterSlowMotion();
			}
		}
		else if (InputExtensions.GetFingerUp())
		{
			AssignNewState(null, true);
			LevelFlowController.Singleton.LeaveSlowMotion();
		}

		_secondaryState?.Execute();
	}

	public static void AssignNewState(InputStateBase newState, bool shouldChangeSecondaryState = false)
	{
		if(shouldChangeSecondaryState)
		{
			_secondaryState?.OnExit();
			_secondaryState = newState;
			_secondaryState?.OnEnter();
			return;
		}
		
		_currentInputState?.OnExit();
		_currentInputState = newState;
		_currentInputState?.OnEnter();
	}

	public void StartDriving()
	{
		AssignNewState(_movementState);
	}

	public void MakeDrawPersistent()
	{
		_isPersistentDrawingLevel = true;
	}

	private void OnDriveEnd()
	{
		AssignNewState(_shootingState);
		AssignNewState(_endState, true);
	}

	private void OnLevelEnd(Faction loser)
	{
		AssignNewState(_endState);
	}
}