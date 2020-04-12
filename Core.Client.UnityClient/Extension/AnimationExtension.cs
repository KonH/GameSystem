using System.Threading.Tasks;
using Core.Client.UnityClient.Threading;
using UnityEngine;

namespace Core.Client.UnityClient.Extension {
	public static class AnimationExtension {
		public static async Task Wait(this Animation anim) {
			anim.Play();
			while ( true ) {
				if ( !anim.isPlaying ) {
					break;
				}
				await Async.WaitForUpdate;
			}
		}

		public static async Task Wait(this Animation anim, AnimationClip clip) {
			anim.clip = clip;
			anim.Play();
			while ( true ) {
				if ( !anim.isPlaying ) {
					break;
				}
				await Async.WaitForUpdate;
			}
		}
	}
}