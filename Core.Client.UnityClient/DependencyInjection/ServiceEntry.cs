using System;

namespace Core.Client.UnityClient.DependencyInjection {
	sealed class ServiceEntry {
		public Type   ImplementationType;
		public object Instance;

		public ServiceEntry(Type implementationType, object instance) {
			ImplementationType = implementationType;
			Instance           = instance;
		}
	}
}