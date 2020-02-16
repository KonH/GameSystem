using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace BuildPipeline {
	[UnsetVisualStudioEnvironmentVariables]
	class Build : NukeBuild {
		public static int Main() => Execute<Build>(x => x.TestCommonDotNet);

		[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
		public readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

		[Parameter("Project to work with")] public readonly string TargetProject;

		[Parameter("Project to test")] public readonly string TestProject;

		[Parameter("Optional runtime to publish")]
		public readonly string TargetRuntime;

		[Parameter("Self-contained for publish")]
		public readonly bool? SelfContained;

		[Parameter("Path to Pi deploy directory on local machine")]
		public readonly string LocalPiHome;

		[Parameter("Service to work with")] public readonly string ServiceName;

		[Parameter("Ssh host for service management")]
		public readonly string SshHost;

		[Parameter("Ssh user for service management")]
		public readonly string SshUserName;

		[Parameter("Ssh password for service management")]
		public readonly string SshPassword;

		[Parameter("Unity build target")] public readonly string TargetBuildTarget;

		Target CleanDotNet => _ => _
			.Requires(() => TargetProject)
			.Executes(() => {
				DotNetClean(new DotNetCleanSettings()
					.SetProject(TargetProject)
					.SetConfiguration(Configuration));
			});

		Target RestoreDotNet => _ => _
			.Requires(() => TargetProject)
			.Executes(() => {
				DotNetRestore(new DotNetRestoreSettings()
					.SetProjectFile(TargetProject));
			});

		Target CompileDotNet => _ => _
			.Requires(() => TargetProject)
			.DependsOn(CleanDotNet, RestoreDotNet)
			.Executes(() => {
				DotNetBuild(new DotNetBuildSettings()
					.SetProjectFile(TargetProject)
					.SetConfiguration(Configuration));
			});

		Target TestDotNet => _ => _
			.Executes(() => {
				if ( string.IsNullOrEmpty(TestProject) ) {
					Logger.Info("Skip: no TestProject specified");
					return;
				}
				DotNetTest(new DotNetTestSettings()
					.SetProjectFile(TestProject)
					.SetConfiguration(Configuration));
			});

		Target TestCommonDotNet => _ => _
			.Executes(() => {
				DotNetTest(new DotNetTestSettings()
					.SetProjectFile("Core.Common.Tests")
					.SetConfiguration(Configuration));
			});

		Target PublishDotNet => _ => _
			.Requires(() => TargetProject)
			.DependsOn(CompileDotNet)
			.DependsOn(TestCommonDotNet)
			.DependsOn(TestDotNet)
			.Executes(() => {
				var settings = new DotNetPublishSettings()
					.SetProject(TargetProject)
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
			.Executes(() => {
				var buildConfigurationDir = RootDirectory / TargetProject / "bin" / Configuration;
				var buildDir              = DeployTarget.GetBuildDir(buildConfigurationDir);
				var targetPath            = (AbsolutePath) LocalPiHome / TargetProject;
				var sourceDirPath         = DeployTarget.GetPublishDir(TargetRuntime, buildDir);
				CopyDirectoryRecursively(sourceDirPath, targetPath,
					DirectoryExistsPolicy.Merge, FileExistsPolicy.OverwriteIfNewer);
			});

		Target StopService => _ => _
			.Description("Stop related service")
			.Requires(() => SshHost)
			.Requires(() => SshUserName)
			.Requires(() => SshPassword)
			.Requires(() => ServiceName)
			.Executes(() => {
				var context = new ServiceTarget.ExecutionContext(SshHost, SshUserName, SshPassword);
				ServiceTarget.StopService(context, ServiceName);
			});

		Target StartService => _ => _
			.Description("Start related service")
			.Requires(() => ServiceName)
			.Requires(() => SshHost)
			.Requires(() => SshUserName)
			.Requires(() => SshPassword)
			.Executes(() => {
				var context = new ServiceTarget.ExecutionContext(SshHost, SshUserName, SshPassword);
				ServiceTarget.StartService(context, ServiceName);
			});

		Target BuildUnity => _ => _
			.Description("Build Unity project")
			.Requires(() => TargetProject)
			.Requires(() => TargetBuildTarget)
			.DependsOn(TestDotNet)
			.Executes(() => { UnityTarget.Build(RootDirectory / TargetProject, TargetBuildTarget); });

		Target DeployUnity => _ => _
			.Description("Deploy Unity project build to target Pi static directory")
			.Requires(() => TargetProject)
			.Requires(() => LocalPiHome)
			.DependsOn(BuildUnity)
			.Executes(() => {
				var buildDir   = RootDirectory / TargetProject / "Build";
				var targetPath = (AbsolutePath) LocalPiHome / "Static" / TargetProject;
				CopyDirectoryRecursively(buildDir, targetPath,
					DirectoryExistsPolicy.Merge, FileExistsPolicy.OverwriteIfNewer);
			});
	}
}