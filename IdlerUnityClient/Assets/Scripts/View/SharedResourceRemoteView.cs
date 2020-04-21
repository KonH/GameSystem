using System.Threading.Tasks;
using Core.Client.UnityClient.Component.View;
using Idler.UnityClient.Repository;

namespace Idler.UnityClient.View {
	public sealed class SharedResourceRemoteView : TextView {
		ISharedStateProvider _provider;

		public async Task Init(ISharedStateProvider provider) {
			_provider = provider;
			base.Init();
			await UpdateValue();
		}

		public async Task UpdateValue() {
			var value = await _provider.GetResourceCount();
			UpdateValue(value);
		}
	}
}