using System;
using Nuke.Common.ProjectModel;

namespace BuildPipeline {
	public static class SolutionExtension {
		public static Project GetTargetProject(this Solution solution, string targetProject) {
			var project = solution.GetProject(targetProject);
			if ( project == null ) {
				throw new InvalidOperationException($"Couldn't find project '{targetProject}'");
			}
			return project;
		}
	}
}