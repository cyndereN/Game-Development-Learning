using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;
    private CinemachineVirtualCamera followCamera;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    // 观察者模式，反向注册方法，player生成时告诉GM
    public void RegisterPlayer(CharacterStats player)
	{
        playerStats = player;

        followCamera = FindObjectOfType<CinemachineVirtualCamera>();

		if (followCamera != null)
		{
            followCamera.Follow = playerStats.transform.GetChild(2);
            // followCamera.LookAt = playerStats.transform.GetChild(2);
		}
	}

    public void AddObserver(IEndGameObserver observer)
	{
        endGameObservers.Add(observer);
	}

    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObservers.Remove(observer);
    }

    // 广播
    public void NotifyObservers()
	{
        foreach(var observer in endGameObservers)
		{
            observer.EndNotify();
		}
	}

    public Transform GetEntrance()
	{
        foreach(var item in FindObjectsOfType<TransitionDestination>())
		{
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER)
                return item.transform;
		}
        return null;
	}
}
