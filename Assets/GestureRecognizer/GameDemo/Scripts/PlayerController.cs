using UnityEngine;
using GestureRecognizer;

public enum PlayerState { Alive, Dead, Blinking }

/// <summary>
/// Controls the player behaviour
/// </summary>
public class PlayerController : MonoBehaviour
{
	/// <summary>
	/// Reference of the laser prefab
	/// </summary>
	public GameObject laserPrefab;

	/// <summary>
	/// Reference of the explosion prefab
	/// </summary>
	public GameObject explosionPrefab;

	/// <summary>
	/// Reference of the engines
	/// </summary>
	public GameObject engines;

	/// <summary>
	/// Reference of the renderer of the player ship
	/// </summary>
	public MeshRenderer shipRenderer;

	/// <summary>
	/// Reference of the level controller
	/// </summary>
	public LevelController levelController;

	/// <summary>
	/// Reference of the gesture behaviour
	/// </summary>
	public GestureBehaviour gestureBehaviour;

	/// <summary>
	/// Current player state
	/// </summary>
	private PlayerState playerState;

	/// <summary>
	/// The speed which the player rotates at towards the target
	/// </summary>
	private float angularSpeed = 20f;

	/// <summary>
	/// Current angle of the player
	/// </summary>
	private float angle;

	/// <summary>
	/// The timer which starts after the player loses a life 
	/// </summary>
	private float deathTimer;

	/// <summary>
	/// Timer for controlling the blinking right after the respawn
	/// </summary>
	private float blinkTimer;

	/// <summary>
	/// Current number of blinks
	/// </summary>
	private int numberOfBlinksCurrent;

	/// <summary>
	/// Temp variable for instantiating a laser object
	/// </summary>
	private GameObject laserObject;

	/// <summary>
	/// The target
	/// </summary>
	private Transform target;

	/// <summary>
	/// The vector to the target
	/// </summary>
	private Vector3 targetVector;

	/// <summary>
	/// Temp quaternion for storing the rotation
	/// </summary>
	private Quaternion q;


	void Awake()
	{
		GestureBehaviour.OnGestureRecognition += OnRecognizeShape;
	}


	void Update()
	{
		if (LevelController.gameState == GameState.Playing || LevelController.gameState == GameState.LifeLost)
		{
			if (playerState != PlayerState.Dead)
			{
				if (target != null)
				{
					targetVector = target.position - transform.position;
					angle = Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg;
					q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
					transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * angularSpeed);
				}

				if (playerState == PlayerState.Blinking)
				{
					blinkTimer += Time.deltaTime;

					if (blinkTimer >= Constants.PlayerBlinkTimeout)
					{
						blinkTimer = 0;
						numberOfBlinksCurrent++;
						Blink(!shipRenderer.enabled);

						if (numberOfBlinksCurrent == Constants.PlayerMaxBlinks)
						{
							numberOfBlinksCurrent = 0;
							playerState = PlayerState.Alive;
							Blink(true);
						}
					}
				}

			}
			else
			{
				deathTimer += Time.deltaTime;

				if (deathTimer >= Constants.PlayerDeathTimeout)
				{
					deathTimer = 0;
					playerState = PlayerState.Blinking;
					levelController.RespawnPlayer();
				}
			}
		}
	}


	public void Fire(GameObject target)
	{

		if (playerState != PlayerState.Dead)
		{
			this.target = target.transform;

			laserObject = Instantiate(laserPrefab, this.transform.position, Quaternion.identity) as GameObject;
			laserObject.GetComponent<LaserController>().target = this.target;
			laserObject.name = Strings.Laser;
		}

	}


	public void NullTarget()
	{
		target = null;
	}


	public void StartPlayer()
	{
		playerState = PlayerState.Alive;
		deathTimer = Constants.ZeroDefault;
		blinkTimer = Constants.ZeroDefault;
		numberOfBlinksCurrent = Constants.ZeroDefault;
		Blink(true);
	}


	public void EndPlayer()
	{
		Blink(false);
	}


	void OnRecognizeShape(Gesture g, Result r)
	{
		gestureBehaviour.ClearGesture();
		levelController.Fire(r.Name);
	}


	void OnTriggerEnter(Collider other)
	{
		if (other.name == Strings.Enemy && playerState == PlayerState.Alive)
		{
			KillPlayer();
			other.GetComponent<EnemyController>().KillEnemy(false);
		}

	}


	void KillPlayer()
	{
		levelController.ConfirmPlayerKill();
		Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
		playerState = PlayerState.Dead;
		Blink(false);
	}


	void Blink(bool isOn)
	{
		shipRenderer.enabled = isOn;
		engines.SetActive(isOn);
	}
}