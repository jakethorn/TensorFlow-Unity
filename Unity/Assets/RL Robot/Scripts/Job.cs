using System;
using System.Threading;

namespace Jake.Threading
{
	/// <summary>
	/// A job to be completed.
	/// </summary>
	public class Job
	{
		public Action callback;
		public AutoResetEvent blocker;
	}
}
