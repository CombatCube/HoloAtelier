using UnityEngine;
using System.Collections;

/// <summary>
/// Controls an enemy behaviour
/// </summary>
public class EnemyController : MonoBehaviour
{
	/// <summary>
	/// Sprites are used to display enemy types on the screen
	/// </summary>
	public Sprite[] gestureSprites;

	/// <summary>
	/// Explosion game object
	/// </summary>
	public GameObject explosionObject;

	/// <summary>
	/// Ship itself
	/// </summary>
	public GameObject enemyShip;

	/// <summary>
	/// The sprite renderer which displays the enemy type
	/// </summary>
	public SpriteRenderer enemyShape;

	/// <summary>
	/// Collider
	/// </summary>
	public Collider enemyCollider;

	/// <summary>
	/// The enemy this behaviour is based on
	/// </summary>
	[HideInInspector]
	public Enemy enemy;

	/// <summary>
	/// Reference of the level controller
	/// </summary>
	[HideInInspector]
	public LevelController levelController;

	/// <summary>
	/// Current speed
	/// </summary>
	private float speed;

	/// <summary>
	/// The target vector from current position to player
	/// </summary>
	private Vector3 targetVector;

	/// <summary>
	/// Particle system of the explosion object
	/// </summary>
	private ParticleSystem explosionPS;


	/// <summary>
	/// Define the enemy type, show it on the screen, calculate the target vector and speed
	/// </summary>
	private void Start()
	{
		enemy = new Enemy();
		enemyShape.sprite = gestureSprites[(int)enemy.Type];

		targetVector = (levelController.player.transform.position - transform.position).normalized;
		speed = Constants.EnemySpeed / levelController.enemySpawnRate;
		explosionPS = explosionObject.GetComponent<ParticleSystem>();
	}


	/// <summary>
	/// Go towards player
	/// </summary>
	private void Update()
	{
		transform.position += targetVector * Time.deltaTime * speed;
	}


	/// <summary>
	/// Kill the enemy, score the player if necessary
	/// </summary>
	/// <param name="scoreTheKill"></param>
	public void KillEnemy(bool scoreTheKill = true)
	{
		levelController.ConfirmEnemyKill(gameObject, scoreTheKill);
		explosionObject.SetActive(true);
		enemyShip.SetActive(false);
		enemyCollider.enabled = false;
		StartCoroutine("CheckIfAlive");
	}


	/// <summary>
	/// When the particle system dies, completely destroy this game object
	/// </summary>
	/// <returns></returns>
	IEnumerator CheckIfAlive()
	{
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			if (!explosionPS.IsAlive(true))
			{
				GameObject.Destroy(gameObject);
			}
		}
	}
}