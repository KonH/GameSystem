using System.Threading.Tasks;
using Idler.Common.State;
using Core.Client.UnityClient.Component.View;
using UnityEngine;

namespace Idler.UnityClient.View {
	public sealed class SharedResourceView : AnimatedTextView {
		[SerializeField] AnimatedTextView _incrementText = null;
		[SerializeField] AnimatedTextView _decrementText = null;

		public void Init(GameState state) {
			base.Init();
			_incrementText.Init();
			_decrementText.Init();
			UpdateValue(state);
		}

		public Task AppearValue(int amount) {
			var view = (amount >= 0) ? _incrementText : _decrementText;
			return view.AnimateValue(Mathf.Abs(amount));
		}

		public Task AnimateValue(GameState state) {
			return AnimateValue(state.Resource.SharedResources);
		}

		void UpdateValue(GameState state) {
			UpdateValue(state.Resource.SharedResources);
		}
	}
}