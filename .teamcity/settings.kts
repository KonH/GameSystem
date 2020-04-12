import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.exec
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs

version = "2019.2"

project {
	subProject {
		name = "ResourceMonitor"
		id = RelativeId(name)
		buildType(createSimpleBuildType("ResourceMonitor"))
		buildType(createDeployBuildType("ResourceMonitor", "resource-monitor"))
		buildType(createRestartServiceBuildType("ResourceMonitor", "resource-monitor"))
		buildType(createStopServiceBuildType("ResourceMonitor", "resource-monitor"))
	}
	subProject {
		name = "Core"
		id = RelativeId(name)
		buildType(createSimpleBuildType("Core.Common", "Core.Common.Tests"))
		buildType(createSimpleBuildType("Core.Service", "Core.Service.Tests"))
		buildType(createSimpleBuildType("Core.Client"))
		buildType(createSimpleBuildType("Core.Client.ConsoleClient"))
	}
	subProject {
		name = "Clicker"
		id = RelativeId(name)
		buildType(createSimpleBuildType("Clicker.Common", "Clicker.Tests"))
		buildType(createSimpleBuildType("Clicker.ConsoleClient"))
		buildType(createSimpleBuildType("Clicker.WebService"))
		buildType(createDeployBuildType("Clicker.WebService", "clicker-web-service", "Clicker.Tests"))
		buildType(createUnityBuildType("ClickerUnityClient", "WebGL", "Clicker.Tests"))
		buildType(createUnityDeployType("ClickerUnityClient", "WebGL", "Clicker.Tests"))
	}
	subProject {
		name = "Idler"
		id = RelativeId(name)
		buildType(createSimpleBuildType("Idler.Common", "Idler.Tests"))
		buildType(createSimpleBuildType("Idler.WebService"))
		buildType(createDeployBuildType("Idler.WebService", "idler-web-service", "Idler.Tests"))
		buildType(createUnityBuildType("IdlerUnityClient", "WebGL", "Idler.Tests"))
		buildType(createUnityDeployType("IdlerUnityClient", "WebGL", "Idler.Tests"))
	}
	buildType(createHaltBuildType())
	buildType(createRebootBuildType())
}

fun createSimpleBuildType(projectName: String, testProjectName: String? = null): BuildType {
	return BuildType {
		name = "Build ($projectName)"
		id = RelativeId("Build_${projectName.replace('.', '_')}")

		vcs {
			root(DslContext.settingsRoot)
		}

		steps {
			exec {
				name = "Build"
				path = "nuke"
				arguments = "--target CompileDotNet --targetProject $projectName"
			}
			if ( testProjectName != null ) {
				exec {
					name = "Test"
					path = "nuke"
					arguments = "--target TestDotNet --testProject $testProjectName"
				}
			}
		}

		triggers {
			vcs {
			}
		}
	}
}

fun createDeployBuildType(projectName: String, serviceName: String, testProjectName: String? = null): BuildType {
	return BuildType {
		name = "Deploy ($projectName)"
		id = RelativeId("Deploy_${projectName.replace('.', '_')}")

		vcs {
			root(DslContext.settingsRoot)
		}

		steps {
			exec {
				name = "Stop Service"
				path = "nuke"
				arguments = "--target StopService --service-name $serviceName --sshHost %env.SSH_HOST% --sshUserName %env.SSH_USER_NAME% --sshPassword %env.SSH_PASSWORD%"
			}
			var deployArgs = "--target DeployDotNet --targetProject $projectName --targetRuntime linux-arm --selfContained true --localPiHome %env.LOCAL_PI_DIRECTORY%"
			if ( testProjectName != null ) {
				deployArgs += " --testProject $testProjectName"
			}
			exec {
				name = "Deploy"
				path = "nuke"
				arguments = deployArgs
			}
			exec {
				name = "Start Service"
				path = "nuke"
				arguments = "--target StartService --service-name $serviceName --sshHost %env.SSH_HOST% --sshUserName %env.SSH_USER_NAME% --sshPassword %env.SSH_PASSWORD%"
			}
		}
	}
}

fun createUnityBuildType(projectName: String, buildTarget: String, testProjectName: String? = null): BuildType {
	return BuildType {
		name = "Unity Build ($projectName, $buildTarget)"
		id = RelativeId("Build_${projectName.replace('.', '_')}_$buildTarget")

		vcs {
			root(DslContext.settingsRoot)
		}

		steps {
			var args = "--target BuildUnity --targetProject $projectName --targetBuildTarget $buildTarget"
			if ( testProjectName != null ) {
				args += " --testProject $testProjectName"
			}
			exec {
				name = "Build"
				path = "nuke"
				arguments = args
			}
		}
	}
}

fun createUnityDeployType(projectName: String, buildTarget: String, testProjectName: String? = null): BuildType {
	return BuildType {
		name = "Unity Deploy ($projectName, $buildTarget)"
		id = RelativeId("Deploy_${projectName.replace('.', '_')}_$buildTarget")

		vcs {
			root(DslContext.settingsRoot)
		}

		steps {
			var args = "--target DeployUnity --targetProject $projectName --targetBuildTarget $buildTarget --localPiHome %env.LOCAL_PI_DIRECTORY%"
			if ( testProjectName != null ) {
				args += " --testProject $testProjectName"
			}
			exec {
				name = "Deploy"
				path = "nuke"
				arguments = args
			}
		}
	}
}

fun createRestartServiceBuildType(projectName: String, serviceName: String): BuildType {
	return BuildType {
		name = "Restart ($projectName)"
		id = RelativeId("Restart_${projectName.replace('.', '_')}")

		vcs {
			root(DslContext.settingsRoot)
		}

		steps {
			exec {
				name = "Stop Service"
				path = "nuke"
				arguments = "--target StopService --service-name $serviceName --sshHost %env.SSH_HOST% --sshUserName %env.SSH_USER_NAME% --sshPassword %env.SSH_PASSWORD%"
			}
			exec {
				name = "Start Service"
				path = "nuke"
				arguments = "--target StartService --service-name $serviceName --sshHost %env.SSH_HOST% --sshUserName %env.SSH_USER_NAME% --sshPassword %env.SSH_PASSWORD%"
			}
		}
	}
}

fun createStopServiceBuildType(projectName: String, serviceName: String): BuildType {
	return BuildType {
		name = "Stop ($projectName)"
		id = RelativeId("Stop_${projectName.replace('.', '_')}")

		vcs {
			root(DslContext.settingsRoot)
		}

		steps {
			exec {
				name = "Stop Service"
				path = "nuke"
				arguments = "--target StopService --service-name $serviceName --sshHost %env.SSH_HOST% --sshUserName %env.SSH_USER_NAME% --sshPassword %env.SSH_PASSWORD%"
			}
		}
	}
}

fun createHaltBuildType(): BuildType {
	return BuildType {
		name = "Halt"
		id = RelativeId("Halt")

		vcs {
			root(DslContext.settingsRoot)
		}

		steps {
			exec {
				name = "Halt"
				path = "nuke"
				arguments = "--target Halt --sshHost %env.SSH_HOST% --sshUserName %env.SSH_USER_NAME% --sshPassword %env.SSH_PASSWORD%"
			}
		}
	}
}

fun createRebootBuildType(): BuildType {
	return BuildType {
		name = "Reboot"
		id = RelativeId("Reboot")

		vcs {
			root(DslContext.settingsRoot)
		}

		steps {
			exec {
				name = "Reboot"
				path = "nuke"
				arguments = "--target Reboot --sshHost %env.SSH_HOST% --sshUserName %env.SSH_USER_NAME% --sshPassword %env.SSH_PASSWORD%"
			}
		}
	}
}