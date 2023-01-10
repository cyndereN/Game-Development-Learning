using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
	private Rigidbody targetRb;
	private GameManger gameManager;
	public int pointValue;
	private float minSpeed = 12;
	private float maxSpeed = 16;
	private float maxTorque = 10;
	private float xRange = 4;
	private float ySpawnPos = -6;

	public ParticleSystem explosionParticle;
	// Start is called before the first frame update
	void Start()
	{
		gameManager = GameObject.Find("Game Manager").GetComponent<GameManger>();
		targetRb = GetComponent<Rigidbody>();
		targetRb.AddForce(RamdomForce(), ForceMode.Impulse);
		targetRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
		targetRb.position = RandomSpawnPos();
	}

	Vector3 RamdomForce()
	{
		return Vector3.up * Random.Range(minSpeed, maxSpeed);
	}

	float RandomTorque()
	{
		return Random.Range(-maxTorque, maxTorque);
	}

	Vector3 RandomSpawnPos()
	{
		return new Vector3(Random.Range(-xRange, xRange), ySpawnPos);
	}
	// Update is called once per frame
	void Update()
	{

	}

	private void OnMouseDown()
	{
		if (gameManager.isGameActive)
		{
			Destroy(gameObject);
			Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
			gameManager.UpdateScore(pointValue);
		}

	}

	private void OnTriggerEnter(Collider other)
	{
		Destroy(gameObject);
		if (gameObject.CompareTag("Bad"))
		{
			gameManager.GameOver();
		}
	}
}
