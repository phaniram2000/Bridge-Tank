using DG.Tweening;
using UnityEngine;

public enum ButtonMechanicEnd
{
	Sender,
	Receiver
};

public class ButtonMechanicController : MonoBehaviour
{
	public ButtonMechanicEnd myEnd;
	[SerializeField] private float yPos;

	private bool _pressed;

	private void OnEnable()
	{
		if(myEnd == ButtonMechanicEnd.Sender) return;

		GameEvents.Singleton.buttonMechanic += OnButtonPress;
	}
	
	private void OnDisable()
	{
		if(myEnd == ButtonMechanicEnd.Sender) return;

		GameEvents.Singleton.buttonMechanic -= OnButtonPress;
	}

	private void OnButtonPress()
	{
		if(_pressed) return;

		_pressed = true;
		transform.DOMoveY(yPos, 1f);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(_pressed) return;
		if(myEnd == ButtonMechanicEnd.Receiver) return;
		
		transform.DOLocalMove(Vector3.zero, .5f);
		GameEvents.Singleton.InvokeButtonMechanic();
	}
}