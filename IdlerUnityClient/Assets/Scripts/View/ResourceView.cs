using System.Threading;
using System.Threading.Tasks;
using Idler.Common.State;
using Core.Client.UnityClient.Component.View;
using UnityEngine;

namespace Idler.UnityClient.View {
	public sealed class ResourceView : AnimatedTextView {
		[SerializeField] AnimatedTextView _incrementText = null;
		[SerializeField] AnimatedTextView _decrementText = null;

		public void Init(GameState state) {
			base.Init();
			_incrementText.Init();
			_decrementText.Init();
			UpdateValue(state);
		}

		public Task AppearValue(int amount, CancellationToken cancellationToken) {
			var view = (amount >= 0) ? _incrementText : _decrementText;
			return view.AnimateValue(Mathf.Abs(amount), cancellationToken);
		}

		public Task AnimateValue(GameState state, CancellationToken cancellationToken) {
			return AnimateValue(state.Resource.Resources, cancellationToken);
		}

		void UpdateValue(GameState state) {
			UpdateValue(state.Resource.Resources);
		}
	}
}