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

		protected override void Init() {
			_animation = GetComponent<Animation>();
			base.Init();
		}

		public override void Init(int value) => Init(value.ToString());

		public Task AnimateValue(string value) {
			UpdateValue(value);
			return _animation.Wait();
		}

		public Task AnimateValue(int value) => AnimateValue(value.ToString());

		public async Task AnimateBeforeValue(string value) {
			await _animation.Wait();
			UpdateValue(value);
		}

		public Task AnimateBeforeValue(int value) => AnimateBeforeValue(value.ToString());
	}
}