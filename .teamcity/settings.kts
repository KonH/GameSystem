import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.exec
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs

version = "2019.2"

project {
    subProject {
        name = "ResourceMonitor"
        id = RelativeId(name)
        buildType(DeployResourceMonitor)
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

    triggers {
        vcs {
        }
    }
})
