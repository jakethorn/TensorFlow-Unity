using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Jake.Threading
{
	/// <summary>
	/// Dispatches jobs to main Unity thread.
	/// </summary>
	public class Dispatcher : MonoBehaviour
	{
		private static Thread mainThread = null;
		private static Queue<Job> jobs = new Queue<Job>();
		
		/// <summary>
		/// True if called from the main thread.
		/// </summary>
		public static bool OnMainThread
		{
			get
			{
				return Thread.CurrentThread.Equals(mainThread);
			}
		}
		
		void Awake()
		{
			mainThread = Thread.CurrentThread;
		}

		void Update()
		{
			while (jobs.Count > 0)
			{
				var job = jobs.Dequeue();

				if (job.callback != null)
				{
					job.callback();
				}

				if (job.blocker != null)
				{
					job.blocker.Set();
				}
			}
		}
		
		public static void Instantiate()
		{
			new GameObject("Dispatcher").AddComponent<Dispatcher>();
		}

		/// <summary>
		/// Dispatch job to be completed on the main thread during the next Unity Update.
		/// Job is completed immediately if already on the main thread.
		/// (If this method isn't working, you might not have a Dispatcher in the scene).
		/// </summary>
		public static void AddJob(Action callback, bool block)
		{
			if (OnMainThread && FindObjectOfType<Dispatcher>() == null)
			{
				Instantiate();
			}
			
			if (OnMainThread)
			{
				callback();
			}
			else
			{
				var blocker = block ? new AutoResetEvent(false) : null;

				jobs.Enqueue(new Job()
				{
					callback = callback,
					blocker = blocker
				});

				if (blocker != null)
				{
					blocker.WaitOne();
				}
			}
		}
	}
}
