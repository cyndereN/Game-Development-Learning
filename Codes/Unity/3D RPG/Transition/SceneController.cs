using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
	public GameObject playerPrefab;
	GameObject player;
	NavMeshAgent playerAgent;

	protected override void Awake()
	{
		base.Awake();
		DontDestroyOnLoad(this);
	}
	public void TransitionToDestination(TransitionPoint transitionPoint)
	{
		switch (transitionPoint.transitionType)
		{
			case TransitionPoint.TransitionType.SameScene:
				StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
				break;
			case TransitionPoint.TransitionType.DifferentScene:
				StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
				break;

		}
	}

	IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
	{
		SaveManager.Instance.SavePlayerData();
		if (SceneManager.GetActiveScene().name != sceneName)
		{
			yield return SceneManager.LoadSceneAsync(sceneName); // 在这一帧等待事件完成，之后执行下面命令
			//重新生成
			yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
			SaveManager.Instance.LoadPlayerData();
			yield break;
		}
		else
		{
			player = GameManager.Instance.playerStats.gameObject;
			playerAgent = player.GetComponent<NavMeshAgent>();
			playerAgent.enabled = false;
			player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
			playerAgent.enabled = true;
			yield return null;
		}
	}

	private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
	{
		var entrances = FindObjectsOfType<TransitionDestination>();
		for(int i = 0; i<entrances.Length; i++)
		{
			if (entrances[i].destinationTag == destinationTag)
				return entrances[i];
		}
		return null;
	}
	public void TransitionToMain()
	{
		StartCoroutine(LoadMain());
	}
	public void TransitionToLoadGame()
	{
		StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
	}
	public void TransitionToFirstLevel()
	{
		StartCoroutine(LoadLevel("SampleScene"));
	}

	IEnumerator LoadLevel(string scene)
	{
		if (scene != "")
		{
			yield return SceneManager.LoadSceneAsync(scene);
			yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

			// 保存游戏
			SaveManager.Instance.SavePlayerData();
			yield break;
		}
		
	}

	IEnumerator LoadMain()
	{
		yield return SceneManager.LoadSceneAsync("Hello");
		yield break;

	}
}
