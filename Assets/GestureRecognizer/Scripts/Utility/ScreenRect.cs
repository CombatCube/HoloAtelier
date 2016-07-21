using UnityEngine;

public class ScreenRect
{

    public float xMin;

    public float xMax;

    public float yMin;

    public float yMax;


    public ScreenRect(Vector2 bottomLeft, Vector2 topRight)
    {
        this.xMin = bottomLeft.x;
        this.xMax = topRight.x;
        this.yMin = bottomLeft.y;
        this.yMax = topRight.y;
    }


    public ScreenRect(float xMin, float xMax, float yMin, float yMax)
    {
        this.xMin = xMin;
        this.xMax = xMax;
        this.yMin = yMin;
        this.yMax = yMax;
    }


    public bool Contains(Vector2 point)
    {
        return point.x >= xMin && point.x <= xMax && point.y >= yMin && point.y <= yMax;
    }


    public Vector2 Clamp(Vector2 point)
    {
        return new Vector2(Mathf.Clamp(point.x, xMin, xMax), Mathf.Clamp(point.y, yMin, yMax));
    }
}
