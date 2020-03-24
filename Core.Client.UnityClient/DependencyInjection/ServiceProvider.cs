using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Common.Extension;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Client.UnityClient.DependencyInjection {
	public sealed class ServiceProvider {
		static ServiceProvider _instance;

		public static ServiceProvider Instance {
			get {
				if ( _instance == null ) {
					_instance = new ServiceProvider();
				}

				return _instance;
			}
		}

		readonly Dictionary<Type, ServiceEntry> _services = new Dictionary<Type, ServiceEntry>();

		public void AddService<TInterface>(TInterface instance) where TInterface : class {
			if ( instance == null ) {
				Debug.LogError($"Instance of service {typeof(TInterface)} should exist!");
				return;
			}
			var entry = new ServiceEntry(instance.GetType(), instance);
			_services.Add(typeof(TInterface), entry);
		}

		public void AddService<TService>() {
			var entry = new ServiceEntry(typeof(TService), null);
			_services.Add(typeof(TService), entry);
		}

		public void AddService<TInterface, TService>() where TService : class, TInterface {
			var entry = new ServiceEntry(typeof(TService), null);
			_services.Add(typeof(TInterface), entry);
		}

		public void AddServiceFromResources<TInterface, TService>(string path)
			where TService : ScriptableObject, TInterface {
			var instance = Resources.Load<TService>(path);
			if ( instance == null ) {
				Debug.LogError($"Instance of service {typeof(TService)} not found in Resources at '{path}'");
			}
			var entry = new ServiceEntry(typeof(TService), instance);
			_services.Add(typeof(TInterface), entry);
		}

		public void AddServiceFromResources<TService>(string path) where TService : ScriptableObject =>
			AddServiceFromResources<TService, TService>(path);

		public void AddJsonFromResourcesAsService<TInterface, TService>(string path)
			where TInterface : class where TService : class, TInterface {
			var jsonAsset = Resources.Load<TextAsset>(path);
			if ( jsonAsset == null ) {
				Debug.LogError($"Json of service {typeof(TService)} not found in Resources at '{path}'");
			}
			var json     = jsonAsset.text;
			var instance = JsonConvert.DeserializeObject<TService>(json);
			AddService<TInterface>(instance);
		}

		public void AddJsonFromResourcesAsService<TService>(string path) where TService : class =>
			AddJsonFromResourcesAsService<TService, TService>(path);


		public TService GetService<TService>() where TService : class {
			return (TService) GetService(typeof(TService));
		}

		public TService CreateService<TService>() where TService : class {
			return (TService) CreateService(typeof(TService));
		}

		object GetService(Type type) {
			var entry = _services.GetOrDefault(type);
			if ( entry == null ) {
				Debug.LogError($"Service {type} not found");
				return null;
			}
			if ( entry.Instance == null ) {
				entry.Instance = CreateService(entry.ImplementationType);
			}
			return entry.Instance;
		}

		bool TryGetService(Type type, out object value) {
			var entry = _services.GetOrDefault(type);
			if ( entry == null ) {
				value = null;
				return false;
			}
			if ( entry.Instance == null ) {
				entry.Instance = CreateService(entry.ImplementationType);
			}
			value = entry.Instance;
			return true;
		}

		object CreateService(Type type) {
			try {
				var constructors = type.GetConstructors();
				if ( HasDefaultConstructor(constructors) ) {
					return Activator.CreateInstance(type);
				}
				var constructor = SelectConstructor(type, constructors);
				if ( constructor == null ) {
					return null;
				}
				var parameters = constructor.GetParameters();
				var instances  = new object[parameters.Length];
				for ( var i = 0; i < parameters.Length; i++ ) {
					instances[i] = ResolveParameter(parameters[i]);
				}
				return constructor.Invoke(instances);
			} catch ( Exception e ) {
				Debug.LogError($"Failed to create instance of {type}: {e}");
				return null;
			}
		}

		bool HasDefaultConstructor(ConstructorInfo[] constructors) {
			foreach ( var constructor in constructors ) {
				if ( constructor.GetParameters().Length == 0 ) {
					return true;
				}
			}
			return false;
		}

		ConstructorInfo SelectConstructor(Type type, ConstructorInfo[] constructors) {
			switch ( constructors.Length ) {
				case 0: {
					Debug.LogError($"No constructors found for {type}");
					return null;
				}
				case 1:
					return constructors[0];
				default: {
					Debug.LogError($"More than one constructors found for {type}");
					return null;
				}
			}
		}

		object ResolveParameter(ParameterInfo parameter) {
			if ( parameter.IsOptional ) {
				if ( TryGetService(parameter.ParameterType, out var value) ) {
					return value;
				}
				return parameter.DefaultValue;
			}
			return GetService(parameter.ParameterType);
		}
	}
}