import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.exec
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs

version = "2019.2"

project {
	subProject {
		name = "ResourceMonitor"
		id = RelativeId(name)
		buildType(DeployResourceMonitor)
		buildType(createSimpleBuildType("ResourceMonitor"))
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
	}
}

object DeployResourceMonitor : BuildType({
	name = "Deploy"

	vcs {
		root(DslContext.settingsRoot)
	}

	steps {
		exec {
			name = "Deploy"
			path = "nuke"
			arguments = "--target DeployDotNet --targetProject ResourceMonitor --targetRuntime linux-arm --selfContained true --localPiHome %env.LOCAL_PI_DIRECTORY%"
		}
	}
})

fun createSimpleBuildType(projectName: String, testProjectName: String? = null): BuildType {
	return BuildType({
		name = "Build ($projectName)"
		id = RelativeId(projectName.replace('.', '_'))

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
	})
}
