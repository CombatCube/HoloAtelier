using UnityEngine;
using System;

namespace GestureRecognizer
{
	[Serializable]
    public class Point
	{
		[SerializeField]
		public int StrokeID;

        [SerializeField]
		public Vector2 Position;


		public Point() {
			StrokeID = 0;
			Position = Vector2.zero;
		}


		public Point(int strokeID, Vector2 position) {
			StrokeID = strokeID;
			Position = position;
		}


		public Point(int strokeID, float x, float y) {
			StrokeID = strokeID;
			Position = new Vector2(x, y);
		}


		public override string ToString() {
			return this.StrokeID + "; " + "(" + this.Position.x + ", " + this.Position.y + ")";
		}
	}
}