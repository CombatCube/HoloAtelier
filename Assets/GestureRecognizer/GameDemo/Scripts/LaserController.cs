using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the behaviour of the laser fire
/// </summary>
public class LaserController : MonoBehaviour
{
	/// <summary>
	/// Target of the laser
	/// </summary>
	public Transform target;

	/// <summary>
	/// The vector that leads this laser to target
	/// </summary>
	private Vector3 targetVector;


	/// <summary>
	/// Find the target vector and rotate the laser shot so that it faces the target
	/// </summary>
	private void Start()
	{
		targetVector = (target.position - transform.position).normalized;
		transform.rotation = Quaternion.AngleAxis((Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg) - 90, Vector3.forward);
	}


	/// <summary>
	/// Move the laser towards the target
	/// </summary>
	void Update()
	{
		transform.position += targetVector * Time.deltaTime * Constants.LaserSpeed;
	}


	/// <summary>
	/// Destroy the enemy object when a collision occurs
	/// </summary>
	/// <param name="other"></param>
	void OnTriggerEnter(Collider other)
	{

		if (other.name == Strings.Enemy)
		{
			other.GetComponent<EnemyController>().KillEnemy();
			Destroy(gameObject);
		}

	}

}
