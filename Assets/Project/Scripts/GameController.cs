using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public float shootingInterval = 3f;
	public float shootingSpeed = 2f;
	public GameObject enemyMissilePrefab;
	public GameObject enemyContainer;
	public Player player;
	public float maximumMovingInterval = 0.4f;
	public float minimumMovingInterval = 0.05f;
	public float movingDistance = 0.1f;
	public float horizontalLimit = 2.5f;

	private float movingInterval;
	private float movingDirection = 1;
	private float movingTimer;
	private float shootingTimer;
	private int enemyCount;

	// Use this for initialization
	void Start () {
		movingInterval = maximumMovingInterval;
		shootingTimer = shootingInterval;
		enemyCount = GetComponentsInChildren<Enemy> ().Length;
	}
	
	// Update is called once per frame
	void Update () {
		int currentEnemyCount = GetComponentsInChildren<Enemy> ().Length;

		// Shooting logic.
		shootingTimer -= Time.deltaTime;
		if (currentEnemyCount > 0 && shootingTimer <= 0f) {
			shootingTimer = shootingInterval;

			Enemy[] enemies = GetComponentsInChildren<Enemy> ();
			Enemy randomEnemy = enemies[Random.Range(0, enemies.Length)];

			GameObject missileInstance = Instantiate (enemyMissilePrefab);
			missileInstance.transform.SetParent (transform);
			missileInstance.transform.position = randomEnemy.transform.position;
			missileInstance.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, -shootingSpeed);
			Destroy (missileInstance, 5f);
		}

		// Movement logic.
		movingTimer -= Time.deltaTime;
		if (movingTimer <= 0f) {
			float difficulty = 1f - (float) currentEnemyCount / enemyCount;

			movingInterval = maximumMovingInterval - (maximumMovingInterval - minimumMovingInterval) * difficulty;
			movingTimer = movingInterval;

			enemyContainer.transform.position = new Vector2 (
				enemyContainer.transform.position.x + (movingDistance * movingDirection),
				enemyContainer.transform.position.y
			);

			if (movingDirection > 0) {
				float rightmostPosition = 0f;
				foreach (Enemy enemy in GetComponentsInChildren<Enemy>()) {
					if (enemy.transform.position.x > rightmostPosition) {
						rightmostPosition = enemy.transform.position.x;
					}
				}

				if (rightmostPosition > horizontalLimit) {
					movingDirection *= -1;

					enemyContainer.transform.position = new Vector2 (
						enemyContainer.transform.position.x,
						enemyContainer.transform.position.y - movingDistance
					);
				}
			} else {
				float leftmostPosition = 0f;
				foreach (Enemy enemy in GetComponentsInChildren<Enemy>()) {
					if (enemy.transform.position.x < leftmostPosition) {
						leftmostPosition = enemy.transform.position.x;
					}
				}

				if (leftmostPosition < -horizontalLimit) {
					movingDirection *= -1;

					enemyContainer.transform.position = new Vector2 (
						enemyContainer.transform.position.x,
						enemyContainer.transform.position.y - movingDistance
					);
				}
			}
		}

		if (currentEnemyCount == 0 || player == null) {
			SceneManager.LoadScene ("Game");
		}
	}
}
