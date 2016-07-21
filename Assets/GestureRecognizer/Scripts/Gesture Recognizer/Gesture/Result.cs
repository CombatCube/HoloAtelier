namespace GestureRecognizer
{
	public class Result
	{
		/// <summary>
		/// Name of the gesture.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Score of the gesture, i.e: how similar to the actual gesture
		/// from the library.
		/// </summary>
		public float Score { get; set; }


		public Result() {
			this.Name = "No match";
			this.Score = float.MaxValue;
		}


		public Result(string name, float score) {
			this.Name = name;
			this.Score = score;
		}


		public void Set(string name, float score) {
			this.Name = name;
			this.Score = score;
		}


		public override string ToString() {
			return this.Name + "; " + this.Score;
		}

	}
}