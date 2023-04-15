using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class HelicopterController : MonoBehaviour
{
	[SerializeField] private float endingXPos = 55f;
	
	[Header("Perlin Noise"), SerializeField] private float magnitude;
	[SerializeField] private float rotationMagnitude, recenterLerp;
	
	private Rigidbody _rb;
	private UnitStats _stats;

	private Vector3 _previousPerlin, _previousPerlinRot;

	private void OnEnable()
	{
		GameEvents.Singleton.driveEnd += OnDriveEnd;
	}
	
	private void OnDisable()
	{
		GameEvents.Singleton.driveEnd -= OnDriveEnd;
	}

	private void Start()
	{
		_stats = GetComponent<UnitStats>();
		_rb = GetComponent<Rigidbody>();
		//give weight to helicopter additive layer
		GetComponent<Animator>().SetLayerWeight(1, 1f);
		
		_stats.myType = UnitType.Heli;
	}

	private void Update()
	{
		if(_stats.isDead) return;
		
		PerlinNoise();
		Recenter();
	}

	private void Recenter()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.back), Time.deltaTime * recenterLerp);

		if (transform.position.x - endingXPos > 2f)
			TakePos();
	}

	private void PerlinNoise()
	{
		var perlinY = Mathf.PerlinNoise(0f, Time.time);
		
		var perlin = Vector3.up * perlinY;
		var perlinRot = Vector3.forward * perlinY;
		
		transform.position += (perlin - _previousPerlin) * magnitude;
		transform.rotation *= Quaternion.Euler((perlinRot - _previousPerlinRot) * rotationMagnitude);
		
		_previousPerlin = perlin;
		_previousPerlinRot = perlinRot;
	}
	
	private void OnDriveEnd()
	{
		Invoke(nameof(TakePos), Random.Range(0,1f));
	}
	
	private void TakePos()
	{
		transform.DOMoveX(endingXPos + Random.Range(-3f, 3f), 4f);
	}

	public void HeliDeath(float explosionForce)
	{
		_rb.useGravity = true;
		_rb.isKinematic = false;
		_rb.constraints = RigidbodyConstraints.None;

		_rb.AddForce(Vector3.left * 5f * explosionForce + Vector3.down * explosionForce + Vector3.back * 40f * explosionForce, ForceMode.Impulse);
		_rb.AddTorque(Vector3.up * 180f, ForceMode.Acceleration);
		GetComponent<HeliAudioController>().OnHeliDeath();
	}
}