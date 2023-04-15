using System.Collections;
using System.Linq;
using UnityEngine;

public class TankController : MonoBehaviour
{
	public WheelCollider[] wheelColliders;

	[Header("Movement Torque Values")]
	public bool showDebugValues;
	public AnimationCurve curve;
	public float motorTorque = 1250f, brakeTorque = 1500f, maxRpm = -1f;

	[Header("Appearances")] public Transform[] wheels;
	public Vector3 initialRotation;
	
	[Header("Anti Topple"), SerializeField] private Transform centerOfMass;
	
	[HideInInspector] public Rigidbody rb;
	[HideInInspector] public float initialAngularDrag, initialDrag;

	[Header("Death"), SerializeField] private Collider[] addRigidbodiesHere;
	[SerializeField] private GameObject[] disableThese;
	
	private UnitStats _stats;
	
	private bool _shouldRecenter;
	
	private void OnEnable()
	{
		GameEvents.Singleton.driveStart += OnDriveStart;
		GameEvents.Singleton.levelEnd += OnLevelEnd;
	}

	private void OnDisable()
	{
		GameEvents.Singleton.driveStart -= OnDriveStart;
		GameEvents.Singleton.levelEnd -= OnLevelEnd;
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		_stats = GetComponent<UnitStats>();
	}
	
	private void Start()
	{
		_stats = GetComponent<UnitStats>();
		_stats.myType = UnitType.Tank;

		initialAngularDrag = rb.angularDrag;
		initialDrag = rb.drag;
		
		if(centerOfMass)
			rb.centerOfMass = centerOfMass.localPosition;
		
		rb.isKinematic = true;
	}
	
	private void Update()
	{
		if (!_shouldRecenter) return;
		
		RecenterToZeroY();
	}

	private void OnGUI()
	{
		if(!showDebugValues) return;
		if(_stats.myFaction == Faction.Enemy) return;

		var text = 
			wheelColliders.Aggregate("", (current, wheel) => 
				current + $"motor = {wheel.motorTorque}, brake = {wheel.brakeTorque}, rpm = {wheel.rpm}\n");

		GUI.Label(new Rect(10f, 10f, 400f, 500f), text);
	}

	private void RecenterToZeroY()
	{
		var position = transform.position;
		transform.position = Vector3.Lerp(position, new Vector3(position.x, position.y, 0f), 10f * Time.deltaTime);
		var euler = transform.eulerAngles;
		transform.rotation = Quaternion.Lerp(transform.rotation, 
			Quaternion.Euler(euler.x, 90f, euler.z),
			Time.deltaTime);
	}

	private void OnDriveStart()
	{
		_shouldRecenter = true;
	}

	private void OnLevelEnd(Faction loser)
	{
		_shouldRecenter = false;
	}

	public void TankDeath(float explosionForce)
	{
		StartCoroutine(Explode(explosionForce));
	}
	
	private IEnumerator Explode(float explosionForce)
	{
		foreach (var myCollider in addRigidbodiesHere)
		{
			myCollider.enabled = true;
			myCollider.transform.parent = null;
			
			Rigidbody rigid;
			rigid = myCollider.attachedRigidbody;
			if(!rigid)
				rigid = myCollider.gameObject.AddComponent<Rigidbody>();
			
			rigid.constraints = RigidbodyConstraints.None;
			rigid.AddExplosionForce(explosionForce, transform.position + Vector3.forward * 2f, 10f, 3f, ForceMode.Impulse);
		}

		foreach (var other in disableThese)
			other.SetActive(false);
		
		yield return null;
	}
}
