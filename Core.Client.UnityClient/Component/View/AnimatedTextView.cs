using System.Threading;
using System.Threading.Tasks;
using Core.Client.UnityClient.Extension;
using TMPro;
using UnityEngine;

namespace Core.Client.UnityClient.Component.View {
	[RequireComponent(typeof(TMP_Text))]
	[RequireComponent(typeof(Animation))]
	public class AnimatedTextView : TextView {
		Animation _animation;

		public override void Init(string value) {
			Init();
			UpdateValue(value);
		}

		public override void Init() {
			_animation = GetComponent<Animation>();
			base.Init();
		}

		public override void Init(int value) => Init(value.ToString());

		public Task AnimateValue(string value, CancellationToken cancellationToken) {
			cancellationToken.ThrowIfCancellationRequested();
			UpdateValue(value);
			return _animation.Wait();
		}

		public Task AnimateValue(int value, CancellationToken cancellationToken) => AnimateValue(value.ToString(), cancellationToken);

		public async Task AnimateBeforeValue(string value, CancellationToken cancellationToken) {
			await _animation.Wait();
			cancellationToken.ThrowIfCancellationRequested();
			UpdateValue(value);
		}

		public Task AnimateBeforeValue(int value, CancellationToken cancellationToken) => AnimateBeforeValue(value.ToString(), cancellationToken);
	}
}