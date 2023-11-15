using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private static T instance;
	public static T Instance
	{
		get { return instance; }
	}

	// 只允许继承类访问的虚函数
	protected virtual void Awake()
	{
		if (instance != null)
			Destroy(gameObject);
		else
			instance = (T)this;
	}

	public static bool IsInitialized
	{
		get { return instance != null; }
	}

	protected virtual void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}
