using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Client.UnityClient.Extension;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Client.UnityClient.Window {
	public sealed class WindowManager {
		readonly WindowSettings _settings;

		readonly Queue<GameObject> _windows = new Queue<GameObject>();

		public WindowManager(WindowSettings settings) {
			_settings = settings;
		}

		public async Task Show<T>(string windowName, Func<T, Task> showMethod) {
			await ShowContent();
			var prefab   = _settings.Prefabs.Find(p => p.Key == windowName).Value;
			var instance = Object.Instantiate(prefab, _settings.Content.transform);
			_windows.Enqueue(instance);
			var component = instance.GetComponent<T>();
			await showMethod(component);
			_windows.Dequeue();
			Object.Destroy(instance);
			await HideContent();
		}

		Task ShowContent() {
			if ( _windows.Count > 0 ) {
				return Task.CompletedTask;
			}
			var content = _settings.Content;
			content.SetActive(true);
			var animation = _settings.Animation;
			if ( animation && _settings.ShowAnimation ) {
				return animation.Wait(_settings.ShowAnimation);
			}
			return Task.CompletedTask;
		}

		Task HideContent() {
			if ( _windows.Count > 0 ) {
				return Task.CompletedTask;
			}
			var animation = _settings.Animation;
			if ( animation && _settings.HideAnimation ) {
				return animation.Wait(_settings.HideAnimation);
			}
			_settings.Content.SetActive(false);
			return Task.CompletedTask;
		}
	}
}