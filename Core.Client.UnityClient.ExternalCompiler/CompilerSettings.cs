using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Core.Client.UnityClient.ExternalCompiler {
	public class CompilerSettings : ScriptableObject {
		public List<string> Paths = new List<string>();

		public void AddLibrary(string path) {
			Paths.Add(path);
			Save();
		}

		public void RemoveLibrary(string path) {
			Paths.Remove(path);
			Save();
		}

		void Save() {
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
		}

		public static CompilerSettings GetOrCreate() {
			var settings = TryFindSettings();
			if ( settings ) {
				return settings;
			}
			return CreateDefaultSettings();
		}

		static CompilerSettings TryFindSettings() {
			var paths =
				AssetDatabase.FindAssets($"t:{typeof(CompilerSettings)}")
					.Select(AssetDatabase.GUIDToAssetPath)
					.ToArray();
			if ( paths.Length > 0 ) {
				return AssetDatabase.LoadAssetAtPath<CompilerSettings>(paths[0]);
			}
			return null;
		}

		static CompilerSettings CreateDefaultSettings() {
			var settings = CreateInstance<CompilerSettings>();
			AssetDatabase.CreateAsset(settings, "Assets/CompilerSettings.asset");
			return settings;
		}
	}
}