using UnityEngine;

namespace Core.Client.UnityClient.Threading {
	public sealed class WaitForUpdate : CustomYieldInstruction {
		public override bool keepWaiting => false;
	}
}