using System;
using System.Threading;
using UnityEngine;

namespace Core.Client.UnityClient.Threading {
	static class UnityThread {
		static int                    ThreadId               { get; set; }
		static SynchronizationContext SynchronizationContext { get; set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Install() {
			ThreadId               = Thread.CurrentThread.ManagedThreadId;
			SynchronizationContext = SynchronizationContext.Current;
		}

		public static void Run(Action action) {
			if ( SynchronizationContext.Current == SynchronizationContext ) {
				action();
			} else {
				SynchronizationContext.Post(_ => action(), null);
			}
		}
	}
}