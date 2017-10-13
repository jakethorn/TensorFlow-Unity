
namespace Jake.StringExtensions
{
	/// <summary>
	/// String extension methods.
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Return end of string of length 'count'.
		/// </summary>
		public static string Endstring(this string that, int count)
		{
			return that.Substring(that.Length - count);
		}

		/// <summary>
		/// Remove 'count' characters from the end of string.
		/// </summary>
		public static string RemoveFromEnd(this string that, int count)
		{
			return that.Remove(that.Length - count);
		}

		/// <summary>
		/// Remove 'end' from the end of string if matching.
		/// </summary>
		public static string RemoveFromEnd(this string that, string end)
		{
			if (that.Endstring(end.Length) == end)
			{
				return that.RemoveFromEnd(end.Length);
			}
			else
			{
				return that;
			}
		}
	}
}
