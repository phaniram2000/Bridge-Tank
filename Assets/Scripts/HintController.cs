using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Dreamteck.Splines;
using UnityEngine;

public class HintController : MonoBehaviour
{
	public static HintController Level;

	[SerializeField] private Color disabledColor, enabledColor;
	[SerializeField] private float splineAnimationSpeed;

	private MeshRenderer _renderer;
	private Material _mat;

	private TweenerCore<Color, Color, ColorOptions> _previousTween;

	private void OnEnable()
	{
		GameEvents.Singleton.driveStart += OnDriveStart;
	}
	
	private void OnDisable()
	{
		GameEvents.Singleton.driveStart -= OnDriveStart;
	}

	private void Awake()
	{
		if(Level) Destroy(gameObject);
		Level ??= this;
	}

	private void Start()
	{
		_renderer = GetComponent<MeshRenderer>();
		_mat = _renderer.sharedMaterial;
		
		_mat.color = disabledColor;
	}

	private void Update()
	{
		_mat.mainTextureOffset += Vector2.down * (Time.deltaTime * splineAnimationSpeed);

		if (Input.GetKeyUp(KeyCode.H)) EnableHint();
	}

	public void EnableHint()
	{
		_previousTween?.Kill();
		_previousTween = DOTween.To(() => _mat.color, value => _mat.color = value, enabledColor, .5f);
	}

	public void DisableHint()
	{
		_previousTween?.Kill();
		_previousTween = DOTween.To(() => _mat.color, value => _mat.color = value, disabledColor, .5f);
	}

	private void OnDriveStart()
	{
		DisableHint();
		AudioManager.Only.Play("Button");
	}
}
