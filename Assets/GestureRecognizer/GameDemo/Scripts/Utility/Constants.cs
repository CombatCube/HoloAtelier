using UnityEngine;

public class Constants
{
	/// <summary>
	/// For everything that defaults to zero.
	/// </summary>
	public const int ZeroDefault = 0;

	/// <summary>
	/// Player health.
	/// </summary>
	public const int HealthDefault = 3;

	/// <summary>
	/// Countdown.
	/// </summary>
	public const int CountDownDefault = 3;

	/// <summary>
	/// How fast the countdown at the start will count.
	/// </summary>
	public const float CountDownMultiplier = 1.5f;

	/// <summary>
	/// How many seconds to wait before player starts blinking after death.
	/// </summary>
	public const float PlayerDeathTimeout = 1.5f;

	/// <summary>
	/// Seconds between player's blinks.
	/// </summary>
	public const float PlayerBlinkTimeout = 0.1f;

	/// <summary>
	/// How many times player blinks.
	/// </summary>
	public const int PlayerMaxBlinks = 11;

	/// <summary>
	/// How many seconds between the death of the player and the score screen.
	/// </summary>
	public const float CountDownToEndTimeout = 2f;

	/// <summary>
	/// Min seconds between enemy spawns.
	/// </summary>
	public const float EnemySpawnRateMin = 0.7f;

	/// <summary>
	/// Max seconds between enemy spawns.
	/// </summary>
	public const float EnemySpawnRateMax = 4;

	/// <summary>
	/// Offset from the top that the enemy spawns.
	/// </summary>
	public const float EnemySpawnOffset = 2;

	/// <summary>
	/// How much of the spawn rate will b regained after life loss.
	/// </summary>
	public const float EnemySpawnRateRegain = 1;

	/// <summary>
	/// How fast the spawn rate will go down from max to min.
	/// </summary>
	public const float EnemySpawnRateTimeMultiplier = 0.04f;

	/// <summary>
	/// How fast the enemy will approach the player.
	/// </summary>
	public const float EnemySpeed = 10f;

	/// <summary>
	/// How fast the laser will travel.
	/// </summary>
	public const float LaserSpeed = 30;

}