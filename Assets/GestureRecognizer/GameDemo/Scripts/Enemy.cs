using UnityEngine;

/// <summary>
/// Enemy types are the shapes from the shapes library
/// </summary>
public enum EnemyType { rectangle, triangle, circle };


/// <summary>
/// Enemy has only one property: its type.
/// </summary>
public class Enemy
{
	/// <summary>
	/// Type determines the shape to kill the enemy
	/// </summary>
	public EnemyType Type { get; set; }

	/// <summary>
	/// Save the type as a string
	/// </summary>
	public string TypeString { get; set; }


	/// <summary>
	/// Default constructor constructs a random enemy
	/// </summary>
	public Enemy()
	{
		this.Type = (EnemyType)Random.Range((int)EnemyType.rectangle, (int)EnemyType.circle + 1);
		this.TypeString = this.Type.ToString();
	}


	/// <summary>
	/// Constructs a certain type of enemy
	/// </summary>
	/// <param name="type">Type of the enemy</param>
	public Enemy(EnemyType type)
	{
		this.Type = type;
		this.TypeString = type.ToString();
	}

}