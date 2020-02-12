using System;
using System.IO;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild {
	public static int Main() => Execute<Build>(x => x.DeployDotNet);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Solution] readonly Solution Solution;

	[Parameter("Project to work with")] readonly string TargetProject;

	[Parameter("Optional runtime to publish")] readonly string TargetRuntime;

	[Parameter("Self-contained for publish")] readonly bool? SelfContained;

	[Parameter("Path to Pi deploy directory on local machine")] readonly string LocalPiHome;

	Target CleanDotNet => _ => _
		.Requires(() => TargetProject)
		.Before(RestoreDotNet)
		.Executes(() =>
		{
			DotNetClean(new DotNetCleanSettings()
				.SetProject(GetTargetProject())
				.SetConfiguration(Configuration));
		});

	Target RestoreDotNet => _ => _
		.Requires(() => TargetProject)
		.Executes(() =>
		{
			DotNetRestore(new DotNetRestoreSettings()
				.SetProjectFile(GetTargetProject()));
		});

	Target CompileDotNet => _ => _
		.Requires(() => TargetProject)
		.DependsOn(CleanDotNet)
		.DependsOn(RestoreDotNet)
		.Executes(() =>
		{
			DotNetBuild(new DotNetBuildSettings()
				.SetProjectFile(GetTargetProject())
				.SetConfiguration(Configuration));
		});

	Target TestDotNet => _ => _
		.Executes(() =>
		{
			var testProject = Solution.GetProject("Core.Common.Tests");
			DotNetTest(new DotNetTestSettings()
				.SetProjectFile(testProject)
				.SetConfiguration(Configuration));
		});

	Target PublishDotNet => _ => _
		.Requires(() => TargetProject)
		.DependsOn(CompileDotNet)
		.DependsOn(TestDotNet)
		.Executes(() =>
		{
			var settings = new DotNetPublishSettings()
				.SetProject(GetTargetProject())
				.SetConfiguration(Configuration);
			if ( TargetRuntime != null ) {
				settings = settings.SetRuntime(TargetRuntime);
			}
			if ( SelfContained.GetValueOrDefault() ) {
				settings = settings.SetSelfContained(SelfContained);
				settings = settings.SetArgumentConfigurator(a => a.Add("/p:PublishSingleFile=true"));
			}
			DotNetPublish(settings);
		});

	Target DeployDotNet => _ => _
		.Description("Deploy build result to target Pi directory root")
		.Requires(() => TargetProject)
		.Requires(() => LocalPiHome)
		.DependsOn(PublishDotNet)
		.Executes(() =>
		{
			var project               = GetTargetProject();
			var buildConfigurationDir = project.Directory / "bin" / Configuration;
			var buildDir              = GetBuildDir(buildConfigurationDir);
			var targetPath            = (AbsolutePath) LocalPiHome / TargetProject;
			var sourceDirPath         = GetPublishDir(buildDir);
			CopyDirectoryRecursively(sourceDirPath, targetPath,
				DirectoryExistsPolicy.Merge, FileExistsPolicy.OverwriteIfNewer);
		});

	Project GetTargetProject() {
		var project = Solution.GetProject(TargetProject);
		if ( project == null ) {
			throw new InvalidOperationException($"Couldn't find project '{TargetProject}'");
		}
		return project;
	}

	AbsolutePath GetBuildDir(AbsolutePath buildConfigurationDir) {
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
		return (AbsolutePath) frameworkDirs[0];
	}

	AbsolutePath GetPublishDir(AbsolutePath buildDir) {
		if ( TargetRuntime == null ) {
			return buildDir / "publish";
		}
		return GetPublishDirWithRuntime(buildDir);
	}

	AbsolutePath GetPublishDirWithRuntime(AbsolutePath buildDir) {
		return buildDir / TargetRuntime / "publish";
	}
}