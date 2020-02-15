using System;
using System.IO;
using Nuke.Common.IO;

namespace BuildPipeline {
	public static class DeployTarget {
		public static PathConstruction.AbsolutePath GetBuildDir(
			PathConstruction.AbsolutePath buildConfigurationDir) {
			// project.GetMSBuildProject() failed for some reason with:
			// The expression ""ConsoleWriter.cs".GetPathsOfAllDirectoriesAbove()" cannot be evaluated.
			// Method 'System.String.GetPathsOfAllDirectoriesAbove' not found.
			// /usr/local/share/dotnet/sdk/3.1.100/Roslyn/Microsoft.Managed.Core.targets

			var frameworkDirs = Directory.GetDirectories(buildConfigurationDir);
			if ( (frameworkDirs.Length == 0) ) {
				throw new InvalidOperationException($"No framework directories found at '{buildConfigurationDir}'");
			}
			if ( (frameworkDirs.Length > 1) ) {
				throw new InvalidOperationException(
					$"More than one framework directories found at '{buildConfigurationDir}'");
			}
			return (PathConstruction.AbsolutePath) frameworkDirs[0];
		}

		public static PathConstruction.AbsolutePath GetPublishDir(
			string targetRuntime, PathConstruction.AbsolutePath buildDir) {
			if ( targetRuntime == null ) {
				return buildDir / "publish";
			}
			return GetPublishDirWithRuntime(targetRuntime, buildDir);
		}

		public static PathConstruction.AbsolutePath GetPublishDirWithRuntime(
			string targetRuntime, PathConstruction.AbsolutePath buildDir) {
			return buildDir / targetRuntime / "publish";
		}
	}
}