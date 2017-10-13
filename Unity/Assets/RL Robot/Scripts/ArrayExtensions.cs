using System;
using System.Collections.Generic;

namespace Jake.ArrayExtensions
{
	/// <summary>
	/// Array extension methods.
	/// View with methods collapsed.
	/// </summary>
	public static class ArrayExtensions
	{
		public static TResult[]	Convert				<T, TResult>	(this T[] ts, Func<T, TResult> func)
		{
			var retval = new TResult[ts.Length];
			for (int i = 0; i < ts.Length; ++i)
			{
				retval[i] = func(ts[i]);
			}

			return retval;
		}
		public static bool		Contains			<T>				(this T[] ts, T t)
		{
			try
			{
				ts.IndexOf(t);
				return true;
			}
			catch
			{
				return false;
			}
		}
		public static T			ElementAt			<T>				(this T[] ts, int index)
		{
			if (index >= 0)
			{
				return ts[index];
			}
			else
			{
				return ts[ts.Length + index];
			}
		}
		public static bool		IsEqualTo			<T>				(this T[] a, T[] b)
		{
			return a.IsEqualTo(b, (x, y) => x.Equals(y));
		}
		public static bool		IsEqualTo			<T>				(this T[] a, T[] b, Func<T, T, bool> func)
		{
			if (a.Length != b.Length)
				return false;

			var equal = true;
			for (int i = 0; i < a.Length; ++i)
			{
				equal &= func(a[i], b[i]);
			}

			return equal;
		}
		public static T			First				<T>				(this T[] ts, Func<T, bool> func)
		{
			return ts[ts.IndexOfFirst(func)];
		}
		public static int		IndexOf				<T>				(this T[] ts, T t)
		{
			return ts.IndexOfFirst((a) => a.Equals(t));
		}
		public static int		IndexOfFirst		<T>				(this T[] ts, Func<T, bool> func)
		{
			for (int i = 0; i < ts.Length; ++i)
				if (func(ts[i]))
					return i;

			throw new Exception("Element not found.");
		}
		public static int		IndexOfFirstOrZero	<T>				(this T[] ts, Func<T, bool> func)
		{
			try
			{
				return ts.IndexOfFirst(func);
			}
			catch
			{
				return 0;
			}
		}
		public static int		IndexOfOrZero		<T>				(this T[] ts, T t)
		{
			try
			{
				return ts.IndexOf(t);
			}
			catch
			{
				return 0;
			}
		}
		public static T			Last				<T>				(this T[] ts)
		{
			return ts[ts.Length - 1];
		}
		public static T[]		Where				<T>				(this T[] ts, Func<T, bool> func)
		{
			var retval = new List<T>();
			foreach (var t in ts)
			{
				if (func(t))
				{
					retval.Add(t);
				}
			}

			return retval.ToArray();
		}
	}
}
