using UnityEngine;
using System;
using System.Collections.Generic;

namespace GestureRecognizer
{
    [Serializable, CreateAssetMenu(order = 900)]
	public class GestureLibrary : ScriptableObject
	{
		[SerializeField]
		public List<Gesture> Gestures = new List<Gesture>();


		public Result Recognize(Gesture gesture) {

			Result result = new Result();
            float distance = float.MaxValue;

            // Compare gesture against all others
            for (int i = 0; i < Gestures.Count; i++)
            {
                distance = GreedyCloudMatch(gesture.NormalizedPoints, Gestures[i].NormalizedPoints);

                if (distance < result.Score) 
                {
                    result.Set(Gestures[i].Name, distance);
                }
            }

            // Normalize score
            result.Score = Mathf.Max((2f - result.Score) / 2f, 0f);
			return result;
		}


		public void ResampleGestures()
		{
			List<Gesture> newGestures = new List<Gesture>();

			for (int i = 0; i < Gestures.Count; i++)
			{
				Gesture newGesture = new Gesture(Gestures[i].OriginalPoints, Gestures[i].Name);
				newGestures.Add(newGesture);
			}

			Gestures = newGestures;
			Debug.Log("Gestures in this library have been resampled");
		}


		private float GreedyCloudMatch(Point[] points1, Point[] points2) {
			float e = 0.5f;
			int step = Mathf.FloorToInt(Mathf.Pow(points1.Length, 1.0f - e));
			float minDistance = float.MaxValue;

			for (int i = 0; i < points1.Length; i += step) {
				float distance1 = CloudDistance(points1, points2, i);
				float distance2 = CloudDistance(points2, points1, i);
				minDistance = Mathf.Min(minDistance, Mathf.Min(distance1, distance2));
			}
			return minDistance;
		}


		private float CloudDistance(Point[] points1, Point[] points2, int startIndex) {
			bool[] matched = new bool[points1.Length];
			Array.Clear(matched, 0, points1.Length);

			float sum = 0;
			int i = startIndex;

			do {
				int index = -1;
				float minDistance = float.MaxValue;

				for (int j = 0; j < points1.Length; j++) {
					if (!matched[j]) {
						float distance = Vector2.Distance(points1[i].Position, points2[j].Position);
						if (distance < minDistance) {
							minDistance = distance;
							index = j;
						}
					}
				}

				matched[index] = true;
				float weight = 1.0f - ((i - startIndex + points1.Length) % points1.Length) / (1.0f * points1.Length);
				sum += weight * minDistance;
				i = (i + 1) % points1.Length;

			} while (i != startIndex);

			return sum;
		}
	}
}