using UnityEngine;
 
public class InputExtensions : MonoBehaviour
{
	//make sure you set this somewhere on first touch
	public static float TouchInputDivisor;
	public static bool IsUsingTouch; 

	public static Vector2 GetInputPosition ()
	{
		if (!GetFingerHeld()) return Vector2.zero;

		if (IsUsingTouch)
			return Input.GetTouch(0).position;
		
		return Input.mousePosition;
	}
	
	public static Vector2 GetInputDelta ()
	{
		if (!GetFingerHeld()) return Vector2.zero;

		if (IsUsingTouch)
			return Input.GetTouch(0).deltaPosition / TouchInputDivisor;
		
		return new Vector2( Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
	}

	public static bool GetFingerDown ()
	{
		if (IsUsingTouch) return Input.GetTouch(0).phase == TouchPhase.Began;

		return Input.GetMouseButtonDown(0);
	}

	public static bool GetFingerUp ()
	{	
		if (IsUsingTouch) return Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled;

		return Input.GetMouseButtonUp(0);
	}
	
	public static bool GetFingerHeld ()
	{
		if (!IsUsingTouch) return Input.GetMouseButton(0);
		if (Input.touchCount > 0)
			return Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary;
		
		return false;
	}
}
