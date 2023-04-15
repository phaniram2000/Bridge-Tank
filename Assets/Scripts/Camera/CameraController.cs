using DG.Tweening;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	[SerializeField] private float timeBeforeFocusOnWinner = 3f;
	[SerializeField] private bool hasCameraFollow;

	private FollowCamera _followCamera;
	private Transform _backgroundMeshes;
	private Transform _fightCam, _endCam;
	private Camera _myCam, _guiCam;

	private float _initFov;

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

	private void Start()
	{
		_fightCam = GameObject.FindGameObjectWithTag("FightCam").transform;
		var trial = GameObject.FindGameObjectWithTag("Background");
		if (trial) _backgroundMeshes = trial.transform;

		_myCam = GetComponent<Camera>();
		_guiCam = transform.GetChild(0).GetComponent<Camera>();
		
		_guiCam.fieldOfView = _initFov = _myCam.fieldOfView; 

		if (!hasCameraFollow && TryGetComponent(out _followCamera))
			_followCamera.enabled = false;
	}

	private void OnDriveEnd()
	{
		TransformToCamera(_fightCam.GetComponent<Camera>());
	}

	private void TransformToCamera(Camera cam)
	{
		var x = cam.fieldOfView;
		
		if(_backgroundMeshes)
			_backgroundMeshes.DOMoveX(_backgroundMeshes.position.x + 40f, 1f);

		transform.DOMove(cam.transform.position, 1f);
		transform.DORotateQuaternion(cam.transform.rotation, 1f);
		_myCam.DOFieldOfView(x, 1f);
		_guiCam.DOFieldOfView(x, 1f);
	}

	private void OnLevelEnd(Faction loser)
	{
		Invoke(nameof(WaitAndDo), timeBeforeFocusOnWinner);
	}

	private void WaitAndDo()
	{
		_endCam = GameObject.FindGameObjectWithTag("WinCamera").transform;
		
		transform.DOMove(_endCam.position, 1f);
		transform.DORotateQuaternion(_endCam.rotation, 1f);

		_myCam.DOFieldOfView(_initFov, 1f);
	}
}
