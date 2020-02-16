using System;
using System.IO;

namespace Core.Client.UnityClient.ExternalCompiler {
	public static class PathUtils {
		public static string GetRelativeDirectoryPath(string fullPath, string rootPath) {
			fullPath = EnsureLastDirectorySeparator(fullPath);
			rootPath = EnsureLastDirectorySeparator(rootPath);
			var fullUri = new Uri(fullPath);
			var rootUri = new Uri(rootPath);
			return rootUri.MakeRelativeUri(fullUri).ToString();
		}

		static string EnsureLastDirectorySeparator(string path) {
			var separator = Path.DirectorySeparatorChar.ToString();
			if ( !path.EndsWith(separator) ) {
				path += separator;
			}
			return path;
		}
	}
}