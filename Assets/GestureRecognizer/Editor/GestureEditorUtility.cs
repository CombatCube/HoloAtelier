using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// A class for utility functions
/// </summary>
namespace GestureRecognizer
{
	public class GestureEditorUtility
	{

		public static Color LineColor
		{
			get
			{
				return (EditorGUIUtility.isProSkin) ? ProSkinLineColor : StandardSkinLineColor;
			}
		}

		private static Color StandardSkinLineColor = new Color(0.2f, 0.2f, 0.2f, 1);
		private static Color ProSkinLineColor = new Color(0.8f, 0.8f, 0.8f, 1);


		/// <summary>
		/// Draws a bezier from "from" to "to"
		/// </summary>
		/// <param name="from">Start point of the bezier</param>
		/// <param name="to">End point of the bezier</param>
		public static void DrawBezier(Vector2 from, Vector3 to)
		{
			Handles.DrawBezier(
				from,
				to,
				from,
				to,
				LineColor,
				null,
				4f
			);
		}


		/// <summary>
		/// Draws a gesture into a draw area
		/// </summary>
		/// <param name="g">Gesture to draw</param>
		/// <param name="drawArea">Area to draw the gesture on</param>
		public static void DrawGesture(Gesture g, Rect drawArea)
		{
			Handles.BeginGUI();

			int currentStrokeID = 0;

			for (int i = 0; i < g.OriginalPoints.Length; i++)
			{
				Handles.color = LineColor;

				if (i != g.OriginalPoints.Length - 1)
				{
					if (currentStrokeID == g.OriginalPoints[i + 1].StrokeID)
					{
						GestureEditorUtility.DrawBezier(
							TranslateToDrawArea(g.OriginalPoints[i].Position, drawArea, true),
							TranslateToDrawArea(g.OriginalPoints[i + 1].Position, drawArea, true)
						);
					}
					else
					{
						currentStrokeID++;
					}
				}
			}

			Handles.EndGUI();
		}


		/// <summary>
		/// Draws a point list into a draw area. The difference between this and DrawGesture is that
		/// this method draws a gesture before it is saved as a gesture (hence, draw "point list").
		/// </summary>
		/// <param name="points">Point list to draw</param>
		/// <param name="drawArea">Area to draw the list on</param>
		public static void DrawPointList(List<Point> points, Rect drawArea)
		{
			Handles.BeginGUI();

			int currentStrokeID = 0;

			for (int i = 0; i < points.Count; i++)
			{
				Handles.color = LineColor;

				if (i != points.Count - 1)
				{
					if (currentStrokeID == points[i + 1].StrokeID)
					{
						GestureEditorUtility.DrawBezier(
							TranslateToDrawArea(points[i].Position, drawArea, true),
							TranslateToDrawArea(points[i + 1].Position, drawArea, true)
						);
					}
					else
					{
						currentStrokeID++;
					}
				}
			}

			Handles.EndGUI();
		}


		public static Vector2 TranslateToDrawArea(Vector2 point, Rect drawArea, bool additive = false)
		{
			return point + (additive ? drawArea.position : -drawArea.position);
		}
	} 
}
