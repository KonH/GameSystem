using System;
using System.Collections.Generic;
using System.Threading;
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

		public async Task Show<T>(string windowName, Func<T, CancellationToken, Task> showMethod, CancellationToken cancellationToken) {
			await ShowContent(cancellationToken);
			var prefab = _settings.Prefabs.Find(p => p.Key == windowName).Value;
			cancellationToken.ThrowIfCancellationRequested();
			var instance = Object.Instantiate(prefab, _settings.Content.transform);
			_windows.Enqueue(instance);
			var component = instance.GetComponent<T>();
			await showMethod(component, cancellationToken);
			_windows.Dequeue();
			cancellationToken.ThrowIfCancellationRequested();
			Object.Destroy(instance);
			await HideContent(cancellationToken);
		}

		Task ShowContent(CancellationToken cancellationToken) {
			if ( _windows.Count > 0 ) {
				return Task.CompletedTask;
			}
			cancellationToken.ThrowIfCancellationRequested();
			var content = _settings.Content;
			content.SetActive(true);
			var animation = _settings.Animation;
			if ( animation && _settings.ShowAnimation ) {
				return animation.Wait(_settings.ShowAnimation);
			}
			return Task.CompletedTask;
		}

		Task HideContent(CancellationToken cancellationToken) {
			if ( _windows.Count > 0 ) {
				return Task.CompletedTask;
			}
			var animation = _settings.Animation;
			if ( animation && _settings.HideAnimation ) {
				return animation.Wait(_settings.HideAnimation);
			}
			cancellationToken.ThrowIfCancellationRequested();
			_settings.Content.SetActive(false);
			return Task.CompletedTask;
		}
	}
}