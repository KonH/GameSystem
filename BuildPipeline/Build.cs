using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

namespace BuildPipeline {
	[CheckBuildProjectConfigurations]
	[UnsetVisualStudioEnvironmentVariables]
	class Build : NukeBuild {
		public static int Main() => Execute<Build>(x => x.DeployDotNet);

		[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
		readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

		[Solution] readonly Solution Solution;

		[Parameter("Project to work with")] readonly string TargetProject;

		[Parameter("Project to test")] readonly string TestProject;

		[Parameter("Optional runtime to publish")] readonly string TargetRuntime;

		[Parameter("Self-contained for publish")] readonly bool? SelfContained;

		[Parameter("Path to Pi deploy directory on local machine")] readonly string LocalPiHome;

		[Parameter("Service to work with")] readonly string ServiceName;

		[Parameter("Ssh host for service management")] readonly string SshHost;

		[Parameter("Ssh user for service management")] readonly string SshUserName;

		[Parameter("Ssh password for service management")] readonly string SshPassword;

		Target CleanDotNet => _ => _
			.Requires(() => TargetProject)
			.Before(RestoreDotNet)
			.Executes(() =>
			{
				DotNetClean(new DotNetCleanSettings()
					.SetProject(Solution.GetTargetProject(TargetProject))
					.SetConfiguration(Configuration));
			});

		Target RestoreDotNet => _ => _
			.Requires(() => TargetProject)
			.Executes(() =>
			{
				DotNetRestore(new DotNetRestoreSettings()
					.SetProjectFile(Solution.GetTargetProject(TargetProject)));
			});

		Target CompileDotNet => _ => _
			.Requires(() => TargetProject)
			.DependsOn(CleanDotNet)
			.DependsOn(RestoreDotNet)
			.Executes(() =>
			{
				DotNetBuild(new DotNetBuildSettings()
					.SetProjectFile(Solution.GetTargetProject(TargetProject))
					.SetConfiguration(Configuration));
			});

		Target TestDotNet => _ => _
			.Executes(() =>
			{
				if ( string.IsNullOrEmpty(TestProject) ) {
					Logger.Info("Skip: no TestProject specified");
					return;
				}
				var testProject = Solution.GetProject(TestProject);
				DotNetTest(new DotNetTestSettings()
					.SetProjectFile(testProject)
					.SetConfiguration(Configuration));
			});

		Target TestCommonDotNet => _ => _
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
			.DependsOn(TestCommonDotNet)
			.DependsOn(TestDotNet)
			.Executes(() =>
			{
				var settings = new DotNetPublishSettings()
					.SetProject(Solution.GetTargetProject(TargetProject))
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
				var project               = Solution.GetTargetProject(TargetProject);
				var buildConfigurationDir = project.Directory / "bin" / Configuration;
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
			.Executes(() =>
			{
				var context = new ServiceTarget.ExecutionContext(SshHost, SshUserName, SshPassword);
				ServiceTarget.StopService(context, ServiceName);
			});

		Target StartService => _ => _
			.Description("Start related service")
			.Requires(() => ServiceName)
			.Requires(() => SshHost)
			.Requires(() => SshUserName)
			.Requires(() => SshPassword)
			.Executes(() =>
			{
				var context = new ServiceTarget.ExecutionContext(SshHost, SshUserName, SshPassword);
				ServiceTarget.StartService(context, ServiceName);
			});
	}
}