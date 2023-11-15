using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
	[Header("Stats Info")]
	public int maxHealth;
	public int currentHealth;
	public int baseDefence;
	public int currentDefence;

	[Header("Kill")]
	public int killPoint;

	[Header("Level")]
	public int currentLevel;
	public int maxLevel;
	public int baseExp;
	public int currentExp;
	public float levelBuff;
	public float LevelMultiplier
	{
		get { return 1 + (currentLevel - 1) * levelBuff; }
	}
	public void UpdateExp(int point)
	{
		currentExp += point;

		if (currentExp >= baseExp)
			LevelUp();
	}

	private void LevelUp()
	{
		// 所有想提升的数据方法
		currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
		baseExp += (int)(baseExp * LevelMultiplier);

		maxHealth = (int)(maxHealth * levelBuff);
		currentHealth = maxHealth;
	}
}
