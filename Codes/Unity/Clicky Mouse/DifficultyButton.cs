using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    private Button button;
    private GameManger gameManager;
    public int difficulty;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManger>();
        button = GetComponent<Button>();
        button.onClick.AddListener(SetDifficulty);
    }

	void SetDifficulty()
	{
        Debug.Log(gameObject.name + " was clicked");
        gameManager.StartGame(difficulty);
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
