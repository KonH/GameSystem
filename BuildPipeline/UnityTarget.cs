using System;
using System.IO;
using System.Linq;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using static Nuke.Common.Tooling.ProcessTasks;

namespace BuildPipeline {
	public static class UnityTarget {
		public static void Build(PathConstruction.AbsolutePath projectPath, string buildTarget) {
			var latestCommit = GetLatestCommitHash();
			var version      = GetProjectVersion(projectPath) + "." + latestCommit;
			var method       = "Core.Client.UnityClient.CustomBuildPipeline.CustomBuildPipeline.RunBuildForVersion";
			RunUnity(
				projectPath,
				$"-executeMethod {method} -projectPath {projectPath} -version={version} -buildTarget={buildTarget}");
		}

		static string GetLatestCommitHash() {
			using var process = StartProcess("git", "rev-parse --short HEAD");
			process.WaitForExit();
			return string.Join('\n',
				process.Output
					.Where(o => o.Type == OutputType.Std)
					.Select(o => o.Text));
		}

		static string GetProjectVersion(PathConstruction.AbsolutePath projectPath) {
			return GetStrStartsWith(
				File.ReadAllLines(projectPath / "ProjectSettings/ProjectSettings.asset"),
				"bundleVersion: ");
		}

		static string GetStrStartsWith(string[] lines, string prefix) {
			return lines
				.Select(l => l.Trim())
				.Where(l => l.StartsWith(prefix))
				.Select(l => l.Substring(prefix.Length))
				.First();
		}

		static void RunUnity(PathConstruction.AbsolutePath projectPath, string cmd) {
			var unityVersion = GetRequiredUnityVersion(projectPath);
			var unityPath    = GetUnityPath(unityVersion);
			var fullCmd      = cmd + " -quit -batchmode -nographics -logFile -";

			using var process = StartProcess(unityPath, fullCmd);
			process.WaitForExit();
			process.AssertZeroExitCode();
		}

		static string GetRequiredUnityVersion(PathConstruction.AbsolutePath projectPath) {
			return GetStrStartsWith(
				File.ReadAllLines(projectPath / "ProjectSettings/ProjectVersion.txt"),
				"m_EditorVersion: ");
		}

		static string GetUnityPath(string unityVersion) {
			var variableName = $"UNITY_{unityVersion.Replace('.', '_')}";
			var value = Environment.GetEnvironmentVariable(variableName);
			if ( string.IsNullOrWhiteSpace(value) ) {
				throw new InvalidOperationException($"Variable name '{variableName}' isn't provided!");
			}
			return value;
		}
	}
}