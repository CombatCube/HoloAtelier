using UnityEngine;
using System.Collections;

namespace GestureRecognizer
{
	public class Utility
	{
	
		// Self-explanatory.
		public static bool IsTouchDevice() {
			return Application.platform == RuntimePlatform.Android || 
				Application.platform == RuntimePlatform.IPhonePlayer || 
				Application.platform == RuntimePlatform.WP8Player;
		}


		/// <summary>
		/// Convert the screen point to world point so that the new point can be put
		/// in the correct position for line renderer.
		/// </summary>
		/// <param name="gesturePoint"></param>
		/// <returns></returns>
		public static Vector3 WorldCoordinateForGesturePoint(Vector3 gesturePoint) {
            return Camera.main.ScreenToWorldPoint(new Vector3(gesturePoint.x, gesturePoint.y, 10));
		}

	}
}