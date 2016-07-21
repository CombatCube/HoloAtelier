using UnityEngine;

/// <summary>
/// As the name suggests, it scrolls the background
/// </summary>
public class BackgroundScroller : MonoBehaviour
{
	/// <summary>
	/// Renderer of the background
	/// </summary>
	private Renderer r;

	/// <summary>
	/// Texture offset
	/// </summary>
	private float offset;

	/// <summary>
	/// Scroll speed
	/// </summary>
	private float speed = 0.02f;


	void Start()
	{
		r = GetComponent<Renderer>();
	}


	void Update()
	{
		offset += Time.deltaTime * speed;
		r.material.mainTextureOffset = new Vector2(0, offset);
	}
}
