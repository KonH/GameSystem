using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Core.Client.UnityClient.ExternalCompiler {
	public class ExternalCompilerWindow : EditorWindow {
		const int MaxControlWidth = 80;

		static List<string> _itemsToRemove = new List<string>();

		Compiler         _compiler = new Compiler();
		CompilerSettings _settings = null;

		public void OnGUI() {
			try {
				EnsureSettings();
				TryShowNextResult();
				DrawLibraries();
				DrawFooterControls();
			} catch ( ArgumentException ) {}
		}

		void EnsureSettings() {
			if ( _settings ) {
				return;
			}
			_settings = CompilerSettings.GetOrCreate();
		}

		void TryShowNextResult() {
			var result = _compiler.TryDequeueNextResult();
			if ( result == null ) {
				return;
			}
			var message =
				$"Compilation of {result.Path} is {(result.Success ? "done" : "failed")}:\n{result.Output}";
			if ( result.Success ) {
				Debug.Log(message);
			} else {
				Debug.LogError(message);
			}
			AssetDatabase.Refresh();
		}

		void DrawLibraries() {
			_itemsToRemove.Clear();
			foreach ( var path in _settings.Paths ) {
				GUILayout.BeginHorizontal();
				GUILayout.Label(path);
				if ( _compiler.IsCompiling(path) ) {
					GUILayout.Label("Compiling...", GUILayout.MaxWidth(MaxControlWidth));
				} else if ( GUILayout.Button("Compile", GUILayout.MaxWidth(MaxControlWidth)) ) {
					_compiler.TryCompileLibrary(Directory.GetCurrentDirectory(), path);
				}
				if ( GUILayout.Button("Remove", GUILayout.MaxWidth(MaxControlWidth)) ) {
					_itemsToRemove.Add(path);
				}
				GUILayout.EndHorizontal();
			}
			foreach ( var path in _itemsToRemove ) {
				_settings.RemoveLibrary(path);
			}
		}

		void DrawFooterControls() {
			GUILayout.BeginHorizontal();
			TryCompileAllLibraries();
			TryAddLibrary();
			GUILayout.EndHorizontal();
		}

		void TryCompileAllLibraries() {
			if ( GUILayout.Button("Compile all", GUILayout.MaxWidth(MaxControlWidth)) ) {
				foreach ( var path in _settings.Paths ) {
					_compiler.TryCompileLibrary(Directory.GetCurrentDirectory(), path);
				}
			}
		}

		void TryAddLibrary() {
			if ( GUILayout.Button("Add", GUILayout.MaxWidth(MaxControlWidth)) ) {
				var currentDirectory  = Directory.GetCurrentDirectory();
				var selectedDirectory = EditorUtility.OpenFolderPanel("Select project folder", currentDirectory, null);
				_settings.AddLibrary(selectedDirectory);
			}
		}

		[MenuItem("ExternalCompiler/Setup")]
		public static void ShowWindow() {
			CreateWindow<ExternalCompilerWindow>("ExternalCompiler").Show();
		}
	}
}