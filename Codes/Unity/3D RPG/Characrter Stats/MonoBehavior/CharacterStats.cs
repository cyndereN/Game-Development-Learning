using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
	public event Action<int, int> UpdateHealthBarOnAttack;
	public CharacterData_SO templateData;
	public CharacterData_SO characterData;
	public AttackData_SO attackData;


	[HideInInspector]
	public bool isCritical;

	void Awake()
	{
		if (templateData != null)
			characterData = Instantiate(templateData);
	}

	#region Read from Data_SO
	public int MaxHealth
	{
		get
		{
			if (characterData != null)
				return characterData.maxHealth;
			else return 0;
		}

		set
		{
			characterData.maxHealth = value;  // value是关键字不是变量
		}
	}
	public int CurrentHealth
	{
		get
		{
			if (characterData != null)
				return characterData.currentHealth;
			else return 0;
		}

		set
		{
			characterData.currentHealth = value;  
		}
	}

	public int BaseDefence
	{
		get
		{
			if (characterData != null)
				return characterData.baseDefence;
			else return 0;
		}

		set
		{
			characterData.baseDefence = value;  // value是关键字不是变量
		}
	}
	public int CurrentDefence
	{
		get
		{
			if (characterData != null)
				return characterData.currentDefence;
			else return 0;
		}

		set
		{
			characterData.currentDefence = value;  // value是关键字不是变量
		}
	}
	#endregion

	#region Character Combat

	public void TakeDamage(CharacterStats attacker, CharacterStats defender)
	{
		int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence, 0);
		CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
		if (attacker.isCritical)
		{
			defender.GetComponent<Animator>().SetTrigger("Hit");
		}

		// Update UI，经验
		UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
		if (CurrentHealth <= 0)
			attacker.characterData.UpdateExp(characterData.killPoint);
	}

	public void TakeDamage(int damage, CharacterStats defender)
	{
		int currentDamage = Mathf.Max(damage - defender.CurrentDefence, 0);
		CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
		UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

		if (CurrentHealth <= 0)
			GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
	}

	private int CurrentDamage()
	{
		float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

		if (isCritical)
		{
			coreDamage *= attackData.criticalMultiplier;
			Debug.Log("Critical Damage! " + coreDamage);
		}

		return (int)coreDamage;
	}

	#endregion

}


