using UnityEngine;
using System.Collections;

public static class RectTransformExtension
{
   
    public static ScreenRect GetScreenRect(this RectTransform rectTransform, Canvas canvas)
    {
        
        Vector3[] corners = new Vector3[4];
        Vector3 topLeft = new Vector3();
        Vector3 bottomRight = new Vector3();

        rectTransform.GetWorldCorners(corners);

        /**
         * 1 - top left
         * 2 - top right
         * 3 - bottom right
         * 4 - bottom left
         */
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
        {
            topLeft = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[1]);
            bottomRight = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, corners[3]);
        }
        else
        {
            topLeft = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
            bottomRight = RectTransformUtility.WorldToScreenPoint(null, corners[3]);
        }

        return new ScreenRect(topLeft.x, bottomRight.x, bottomRight.y, topLeft.y);
    }

}