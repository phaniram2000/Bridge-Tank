using UnityEngine;

public class TimepassRagdollController : MonoBehaviour
{
	private Animator _anim;
	private Rigidbody _rb;
	private Collider _collider;
	private bool _isDone, _isStarted;
	
	private void Start()
	{
		_rb = GetComponent<Rigidbody>();
		_anim = GetComponent<Animator>();
		_collider = GetComponent<Collider>();
	}

	private void OnCollisionEnter(Collision other)
	{
		if(_isStarted && _isDone) return;
		if (!other.gameObject.CompareTag("GeneratedMesh") && !other.gameObject.CompareTag("Player")) return;
		
		_anim.enabled = false;
		_rb.isKinematic = false;
		InputHandler.AssignNewState(null);
		GameEvents.Singleton.InvokeLevelEnd(Faction.Player);
		
		if(_isStarted)
		{
			_rb.AddForce(0f, 420f, -250f, ForceMode.Impulse);
			_collider.enabled = false;
			_isDone = true;
		}
		_isStarted = true;
	}
}
