
namespace Jake
{
	/// <summary>
	/// Helpful math related methods.
	/// </summary>
	public static class Math
	{
		/// <summary>
		/// Reduce angle in degrees to be between 0 and 360.
		/// </summary>
		public static float ReduceAngle(float a)
		{
			while (a < 0)
			{
				a += 360;
			}

			while (a > 360)
			{
				a -= 360;
			}

			return a;
		}
	}
}
