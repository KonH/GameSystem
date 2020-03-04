using System.Threading.Tasks;
using Clicker.Common.State;
using Core.Client.UnityClient.Component.View;
using UnityEngine;

namespace Clicker.UnityClient.View {
	public sealed class ResourceView : AnimatedTextView {
		[SerializeField] AnimatedTextView _appearText = null;

		public void Init(GameState state) {
			base.Init();
			_appearText.Init(0);
			UpdateValue(state);
		}

		public Task AppearValue(int amount) {
			return _appearText.AnimateValue(amount);
		}

		public Task AnimateValue(GameState state) {
			return AnimateValue(state.Resource.Resources);
		}

		void UpdateValue(GameState state) {
			UpdateValue(state.Resource.Resources);
		}
	}
}