using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainCanvasController : MonoBehaviour
{
	[SerializeField] private GameObject starL, starM, starR, playPanel, endPanel, endRetryButton, endNextButton;
	[SerializeField] private Text levelNum, endText;
	[SerializeField] private Button driveButton;
	[SerializeField] private float pitchAdder = 0.1f;

	private Queue<GameObject> _starsReceived;
	private float _myPitch = 1f;
	private bool _isHintVisible;

	private Animator _anim;
	
	private static readonly int LevelEnd = Animator.StringToHash("LevelEnd");

	private void OnEnable()
	{
		GameEvents.Singleton.levelEnd += OnLevelEnd;
	}

	private void OnDisable()
	{	
		GameEvents.Singleton.levelEnd -= OnLevelEnd;
	}

	private void Start()
	{
		_anim = GetComponent<Animator>();
		GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("GUI Cam").GetComponent<Camera>();

		SetLevelText();
		
		_starsReceived = new Queue<GameObject>();
		
		if(SceneManager.GetActiveScene().buildIndex > 5) return;
		ToggleHint();
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
			StartDriving();
		if (Input.GetKeyDown(KeyCode.R))
			Retry();
	}

	private void SetLevelText()
	{
		levelNum.text = "Level " +  PlayerPrefs.GetInt("levelNo");
	}

	public void StartDriving()
	{
		InputHandler.Only.StartDriving();
		driveButton.interactable = false;
		GameEvents.Singleton.InvokeDriveStart();
		
		Vibration.Vibrate(15);
	}

	public void Retry()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		AudioManager.Only.Play("Retry");
	}

	public void NextLevel()
	{
		AudioManager.Only.Play("Next");
		if (PlayerPrefs.GetInt("levelNo") < SceneManager.sceneCountInBuildSettings - 1)
		{
			var x = PlayerPrefs.GetInt("levelNo") + 1;
			SceneManager.LoadScene(x);
			PlayerPrefs.SetInt("lastBuildIndex", x);
		}
		else
		{
			var x = Random.Range(5, SceneManager.sceneCountInBuildSettings - 1);
			SceneManager.LoadScene(x);
			PlayerPrefs.SetInt("lastBuildIndex", x);
		}
		PlayerPrefs.SetInt("levelNo", PlayerPrefs.GetInt("levelNo") + 1);
	}

	private void OnLevelEnd(Faction loser)
	{
		playPanel.SetActive(false);
		StartCoroutine(ShowEndCanvas(loser));
	}

	private IEnumerator ShowEndCanvas(Faction loser)
	{
		yield return new WaitForSeconds(1.5f);
		
		_anim.SetTrigger(LevelEnd);
		if (loser == Faction.Enemy)
		{
			StartCoroutine(ShowStars(loser));
			endText.text = "Level Clear!";
			endNextButton.SetActive(true);
			endRetryButton.SetActive(false);
			yield break;
		}
		
		endText.text = "Level Failed!";
		endNextButton.SetActive(false);
		endRetryButton.SetActive(true);
	}

	private IEnumerator ShowStars(Faction loser)
	{
		yield return new WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).IsName("EndPanel"));

		endPanel.SetActive(true);
		if (loser == Faction.Player) yield break;

		_starsReceived.Enqueue(starL);
		_starsReceived.Enqueue(starR);
		_starsReceived.Enqueue(starM);
	}

	public void AnimationCalledShowStar()
	{
		if(_starsReceived.Count == 0) return;
		_myPitch += pitchAdder;
		_starsReceived.Dequeue().SetActive(true);
		AudioManager.Only.Play("GetStar", _myPitch);
	}

	public void ToggleHint()
	{
		if (_isHintVisible)
			HintController.Level.DisableHint();
		else
			HintController.Level.EnableHint();

		_isHintVisible = !_isHintVisible;
	}
}